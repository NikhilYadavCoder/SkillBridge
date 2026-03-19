using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SkillBridge.DTOs.Resume;
using SkillBridge.Models.AI;

namespace SkillBridge.Services.AI
{
    public class GroqService : IGroqService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GroqService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ResumeParsedDto> ExtractResumeDataAsync(string resumeText)
        {
            try
            {
                var apiKey = _configuration["Groq:ApiKey"];
                if (string.IsNullOrEmpty(apiKey) || apiKey == "your-groq-api-key-here")
                {
                    return ExtractFromResumeText(resumeText);
                }

                var prompt = $"You are a resume parser. Extract information and return ONLY valid JSON with no explanations.\n\n" +
                    "Rules:\n" +
                    "1. Return ONLY the JSON object - no preamble, no markdown\n" +
                    "2. Extract ONLY information that exists in the resume\n" +
                    "3. Do NOT make up data or infer anything\n" +
                    "4. Use default values for missing sections\n\n" +
                    "JSON structure:\n" +
                    "{{\n" +
                    "  \"skills\": [],\n" +
                    "  \"experience\": [],\n" +
                    "  \"education\": [],\n" +
                    "  \"projects\": [],\n" +
                    "  \"certifications\": []\n" +
                    "}}\n\n" +
                    "EXTRACTION:\n\n" +
                    "SKILLS: From TECHNICAL SKILLS or Languages section only. If missing, return [].\n\n" +
                    "EXPERIENCE (STRICT - ONLY FROM DEDICATED SECTIONS):\n" +
                    "- ONLY extract from explicit EXPERIENCE or WORK EXPERIENCE section\n" +
                    "- Must have BOTH: Job role AND Company name\n" +
                    "- Do NOT guess, infer, or create experience\n" +
                    "- Do NOT convert projects into experience\n" +
                    "- Ignore all PROJECTS, ACADEMIC WORK, COURSEWORK sections\n" +
                    "- If no valid experience found: return [\"No experience available\"]\n" +
                    "- Format each entry as: Job Title - Company Name\n\n" +
                    "EDUCATION: From EDUCATION section only\n" +
                    "- Format: Degree in Major - Institution\n" +
                    "- Remove years and CGPA\n" +
                    "- If missing, return []\n\n" +
                    "PROJECTS: From PROJECTS section only\n" +
                    "- Just project names, remove descriptions and dates\n" +
                    "- If missing, return []\n\n" +
                    "CERTIFICATIONS: From CERTIFICATIONS section only\n" +
                    "- Not LeetCode or CodeChef rankings\n" +
                    "- If missing: [\"No certifications available\"]\n\n" +
                    $"Resume:\n{resumeText}\n\n" +
                    "Return only JSON:";

                var request = new GroqRequest
                {
                    model = "llama-3.1-8b-instant",
                    messages = new List<Message>
                    {
                        new Message { role = "user", content = prompt }
                    },
                    temperature = 0.2
                };

                var requestBody = JsonSerializer.Serialize(request);
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    return ExtractFromResumeText(resumeText);
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var groqResponse = JsonSerializer.Deserialize<GroqResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (groqResponse?.choices != null && groqResponse.choices.Count > 0)
                {
                    var jsonResponse = groqResponse.choices[0].message.content;

                    // Clean the AI response before deserialization
                    var cleanedJson = jsonResponse.Trim();
                    
                    // Remove markdown code blocks
                    cleanedJson = System.Text.RegularExpressions.Regex.Replace(cleanedJson, @"```json\s*", "");
                    cleanedJson = System.Text.RegularExpressions.Regex.Replace(cleanedJson, @"```\s*", "");
                    
                    // Extract content between first { and last }
                    var firstBrace = cleanedJson.IndexOf('{');
                    var lastBrace = cleanedJson.LastIndexOf('}');
                    
                    if (firstBrace != -1 && lastBrace != -1 && lastBrace > firstBrace)
                    {
                        cleanedJson = cleanedJson.Substring(firstBrace, lastBrace - firstBrace + 1);
                    }
                    
                    cleanedJson = cleanedJson.Trim();

                    var parsedData = JsonSerializer.Deserialize<ResumeParsedDto>(cleanedJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return parsedData ?? ExtractFromResumeText(resumeText);
                }

                return ExtractFromResumeText(resumeText);
            }
            catch
            {
                return ExtractFromResumeText(resumeText);
            }
        }

        private ResumeParsedDto GetFallbackData()
        {
            // Return truly empty fallback when no resume text is provided
            return new ResumeParsedDto
            {
                skills = new(),
                experience = new() { "No experience available" },
                education = new(),
                projects = new(),
                certifications = new() { "No certifications available" }
            };
        }

        private ResumeParsedDto ExtractFromResumeText(string resumeText)
        {
            if (string.IsNullOrWhiteSpace(resumeText))
            {
                return GetFallbackData();
            }

            // STEP 1: Text normalization
            var normalizedText = NormalizeResumeText(resumeText);

            var result = new ResumeParsedDto
            {
                skills = ExtractSkills(normalizedText),
                experience = ExtractExperience(normalizedText),
                education = ExtractEducation(normalizedText),
                projects = ExtractProjects(normalizedText),
                certifications = ExtractCertifications(normalizedText)
            };

            return result;
        }

        private string NormalizeResumeText(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Insert space before capital letters (handles compressed text like IndianInstitute)
            var sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0 && char.IsUpper(text[i]) && char.IsLower(text[i - 1]))
                {
                    sb.Append(' ');
                }
                sb.Append(text[i]);
            }

            text = sb.ToString();

            // Replace special characters with spaces
            text = text.Replace(":", " ");
            text = text.Replace("|", " ");
            text = text.Replace("•", " ");
            text = text.Replace("-", " ");
            text = text.Replace(",", " ");
            
            // Normalize multiple spaces to single space
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");
            
            return text.Trim();
        }

        private List<string> ExtractSkills(string resumeText)
        {
            var skills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            // Whitelist of valid technical skills
            var validTechSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // Programming Languages
                "C", "C++", "C#", "Java", "Python", "JavaScript", "TypeScript", "PHP", "Ruby", "Go", "Rust",
                "R", "Kotlin", "Swift", "Objective-C", "Scala", "Groovy", "Perl", "MATLAB",
                
                // Frontend / Web
                "React", "Vue", "Angular", "Svelte", "Next.js", "Nuxt", "Gatsby", "HTML", "CSS", "SCSS",
                "Tailwind", "Bootstrap", "Material-UI", "Redux", "Vuex", "State Management",
                
                // Backend / Frameworks
                "ASP.NET", "ASP.NET Core", ".NET", "Node.js", "Express", "Django", "Flask", "Spring",
                "Spring Boot", "Spring Cloud", "Fastapi", "Rails", "Laravel", "Symfony",
                
                // Databases
                "SQL", "MySQL", "PostgreSQL", "MongoDB", "Redis", "SQLite", "SQL Server", "Oracle",
                "Firebase", "Cassandra", "Elasticsearch", "DynamoDB", "MariaDB",
                
                // DevOps / Cloud / Infrastructure
                "Docker", "Kubernetes", "AWS", "Azure", "GCP", "Terraform", "Jenkins", "CI/CD",
                "Lambda", "EC2", "S3", "RDS", "CloudFront", "GitLab CI", "GitHub Actions",
                
                // Version Control
                "Git", "GitHub", "GitLab", "Bitbucket", "SVN",
                
                // Tools / Libraries
                "GraphQL", "REST", "RESTful API", "Microservices", "SOAP", "JSON", "XML",
                "JWT", "OAuth", "LDAP", "OpenSSL",
                
                // Testing / Quality
                "JUnit", "Pytest", "Jest", "Mocha", "Selenium", "Cypress", "TestNG",
                
                // Mobile
                "iOS", "Android", "React Native", "Flutter", "Xamarin",
                
                // Other Technologies
                "Linux", "Windows", "macOS", "Unix", "Agile", "Scrum", "Kanban",
                "Machine Learning", "TensorFlow", "PyTorch", "Keras", "Scikit-learn"
            };

            // Normalize the text first
            var normalizedText = NormalizeResumeText(resumeText);
            var textUpper = normalizedText.ToUpper();

            // Extract from dedicated SKILLS section first
            var skillsIndex = textUpper.IndexOf("TECHNICAL SKILLS");
            if (skillsIndex == -1)
                skillsIndex = textUpper.IndexOf("SKILLS");
            
            if (skillsIndex != -1)
            {
                var nextSectionIndex = FindNextSectionIndex(textUpper, skillsIndex + 1);
                var skillsSection = normalizedText.Substring(skillsIndex, 
                    nextSectionIndex == -1 ? normalizedText.Length - skillsIndex : nextSectionIndex - skillsIndex);
                
                ExtractSkillsFromText(skillsSection, skills, validTechSkills);
            }

            // Extract from PROJECTS section
            var projectsIndex = textUpper.IndexOf("PROJECTS");
            if (projectsIndex != -1)
            {
                var nextSectionIndex = FindNextSectionIndex(textUpper, projectsIndex + 1);
                var projectsSection = normalizedText.Substring(projectsIndex,
                    nextSectionIndex == -1 ? normalizedText.Length - projectsIndex : nextSectionIndex - projectsIndex);
                
                ExtractSkillsFromText(projectsSection, skills, validTechSkills);
            }

            // Extract from EXPERIENCE section
            var experienceIndex = textUpper.IndexOf("WORK EXPERIENCE");
            if (experienceIndex == -1)
                experienceIndex = textUpper.IndexOf("EXPERIENCE");
            
            if (experienceIndex != -1)
            {
                var nextSectionIndex = FindNextSectionIndex(textUpper, experienceIndex + 1);
                var experienceSection = normalizedText.Substring(experienceIndex,
                    nextSectionIndex == -1 ? normalizedText.Length - experienceIndex : nextSectionIndex - experienceIndex);
                
                ExtractSkillsFromText(experienceSection, skills, validTechSkills);
            }

            // Remove known non-tech words
            var nonTechWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "SKILLS", "TECHNICAL", "LANGUAGES", "FRAMEWORKS", "TECHNOLOGIES", "TOOLS",
                "DEVELOPER", "DEVELOPERTOOLS", "TYPE", "SCRIPT", "WINDSURF", "HUB",
                "JS", "DOTNET", "COMMUNICATION", "LEADERSHIP", "TEAMWORK", "PROBLEM",
                "SOLVING", "ANALYTICAL", "CRITICAL", "THINKING"
            };

            foreach (var word in nonTechWords)
            {
                skills.Remove(word);
            }

            return skills.ToList();
        }

        private void ExtractSkillsFromText(string text, HashSet<string> skills, HashSet<string> validTechSkills)
        {
            if (string.IsNullOrEmpty(text)) return;

            // Split by multiple delimiters and collect candidates
            var candidates = new List<string>();

            // Split by common delimiters
            var parts = System.Text.RegularExpressions.Regex.Split(text, @"[,\s|+;]");
            
            foreach (var part in parts)
            {
                var cleaned = part.Trim()
                    .Replace("•", "")
                    .Replace("*", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Trim();

                if (string.IsNullOrWhiteSpace(cleaned) || cleaned.Length < 2)
                    continue;

                // Normalize common variations
                cleaned = NormalizeTechName(cleaned);

                // Check against whitelist
                if (validTechSkills.Contains(cleaned))
                {
                    skills.Add(cleaned);
                }
            }

            // Also look for "Technology: X, Y, Z" patterns
            var techPatterns = new[] { "Technology:", "Technologies:", "Built using", "Using", "Stack:", "Tools:" };
            foreach (var pattern in techPatterns)
            {
                var patternIndex = text.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
                if (patternIndex != -1)
                {
                    // Get text after the pattern until next newline or section
                    var startIndex = patternIndex + pattern.Length;
                    var endIndex = text.IndexOf('\n', startIndex);
                    if (endIndex == -1) endIndex = text.Length;

                    var techText = text.Substring(startIndex, endIndex - startIndex);
                    var techParts = System.Text.RegularExpressions.Regex.Split(techText, @"[,|+]");
                    
                    foreach (var tech in techParts)
                    {
                        var cleaned = tech.Trim();
                        cleaned = NormalizeTechName(cleaned);
                        
                        if (!string.IsNullOrWhiteSpace(cleaned) && validTechSkills.Contains(cleaned))
                        {
                            skills.Add(cleaned);
                        }
                    }
                }
            }
        }

        private string NormalizeTechName(string tech)
        {
            if (string.IsNullOrWhiteSpace(tech)) return tech;

            tech = tech.Trim();

            // Normalize common variations
            tech = System.Text.RegularExpressions.Regex.Replace(tech, @"ReactJS", "React", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tech = System.Text.RegularExpressions.Regex.Replace(tech, @"Vue\.?js", "Vue", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tech = System.Text.RegularExpressions.Regex.Replace(tech, @"Node\.?js", "Node.js", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tech = System.Text.RegularExpressions.Regex.Replace(tech, @"Dotnet|\.NET Core", ".NET", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tech = System.Text.RegularExpressions.Regex.Replace(tech, @"MS SQL|MSSQL", "SQL Server", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tech = System.Text.RegularExpressions.Regex.Replace(tech, @"Github", "GitHub", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tech = System.Text.RegularExpressions.Regex.Replace(tech, @"TypeScript", "TypeScript", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tech = System.Text.RegularExpressions.Regex.Replace(tech, @"C\+\+", "C++", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            tech = System.Text.RegularExpressions.Regex.Replace(tech, @"C#|Csharp", "C#", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Remove trailing punctuation
            tech = System.Text.RegularExpressions.Regex.Replace(tech, @"[,\.\;:!?]+$", "");

            return tech.Trim();
        }

        private bool ContainsManyCapitals(string text)
        {
            var capitalCount = text.Count(char.IsUpper);
            return capitalCount > text.Length / 2;
        }

        private List<string> ExtractExperience(string resumeText)
        {
            var experience = new List<string>();
            var textUpper = resumeText.ToUpper();
            
            // ONLY extract if explicit EXPERIENCE or WORK EXPERIENCE section exists
            var experienceIndex = textUpper.IndexOf("WORK EXPERIENCE");
            if (experienceIndex == -1)
                experienceIndex = textUpper.IndexOf("EXPERIENCE");
            
            if (experienceIndex == -1)
                return new() { "No experience available" };

            // Get text until next section
            var nextSectionIndex = FindNextSectionIndex(textUpper, experienceIndex + 1);
            var experienceSection = resumeText.Substring(experienceIndex,
                nextSectionIndex == -1 ? resumeText.Length - experienceIndex : nextSectionIndex - experienceIndex);

            // Extract lines that likely contain job role + company
            var lines = experienceSection.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                var cleaned = line.Trim()
                    .Replace("•", "")
                    .Replace("-", "")
                    .Replace("*", "")
                    .Trim();

                // Skip section headers and keywords
                if (cleaned.Equals("EXPERIENCE", StringComparison.OrdinalIgnoreCase) ||
                    cleaned.Equals("WORK EXPERIENCE", StringComparison.OrdinalIgnoreCase) ||
                    string.IsNullOrWhiteSpace(cleaned))
                    continue;

                // Look for patterns like "Role - Company" or "Role at Company"
                if ((cleaned.Contains(" - ") || cleaned.Contains(" at ")) && cleaned.Length > 5)
                {
                    // Must contain both a role word and company indicator
                    var hasRoleWord = ContainsAny(cleaned, new[] { "developer", "engineer", "manager", "analyst", "architect", "designer", "intern", "lead", "specialist", "consultant", "director", "coordinator", "officer" });
                    
                    if (hasRoleWord)
                    {
                        experience.Add(cleaned);
                    }
                }
            }

            return experience.Count > 0 ? experience : new() { "No experience available" };
        }

        private List<string> ExtractEducation(string resumeText)
        {
            var education = new List<string>();
            var textUpper = resumeText.ToUpper();
            
            var educationIndex = textUpper.IndexOf("EDUCATION");
            if (educationIndex == -1)
                return education;

            // Get text until next section
            var nextSectionIndex = FindNextSectionIndex(textUpper, educationIndex + 1);
            var educationSection = resumeText.Substring(educationIndex,
                nextSectionIndex == -1 ? resumeText.Length - educationIndex : nextSectionIndex - educationIndex);

            // Normalize the section to handle compressed text
            educationSection = NormalizeResumeText(educationSection);

            var lines = educationSection.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                var cleaned = line.Trim()
                    .Replace("•", "")
                    .Replace("*", "")
                    .Trim();

                if (cleaned.Equals("EDUCATION", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(cleaned))
                    continue;

                // Look for degree keywords
                var hasDegreeKeyword = ContainsAny(cleaned, new[] { 
                    "bachelor", "master", "degree", "diploma", "phd", "associate",
                    "b.s", "b.a", "b.tech", "be", "bca", "mca",
                    "m.s", "m.tech", "ma", "mba", "btech", "mtech"
                });

                if (hasDegreeKeyword)
                {
                    // Remove years (2022, 2026, etc.)
                    cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\b\d{4}\b", "");
                    cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"–\s*\d{4}", "");
                    
                    // Remove CGPA/GPA
                    cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"CGPA:\s*\d+\.?\d*", "");
                    cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"GPA:\s*\d+\.?\d*", "");
                    cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"CGPA\s*\d+\.?\d*", "");
                    
                    // Remove coursework and other metadata
                    cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"Coursework:.*?(?=,|$)", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    
                    // Normalize multiple spaces
                    cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\s+", " ");
                    cleaned = cleaned.Trim();

                    if (!string.IsNullOrWhiteSpace(cleaned) && cleaned.Length > 5)
                    {
                        // Format as "Degree - Institute"
                        // Try to find the dash/separator and clean it up
                        if (!cleaned.Contains(" - "))
                        {
                            // Add dash if degree and institute are separate
                            var degreeMatch = System.Text.RegularExpressions.Regex.Match(cleaned, 
                                @"(B\.?Tech|B\.?S|B\.?A|M\.?Tech|M\.?S|M\.?Tech|Bachelor|Master|Diploma|PhD)", 
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            
                            if (degreeMatch.Success && degreeMatch.Index > 0)
                            {
                                var beforeDegree = cleaned.Substring(0, degreeMatch.Index).Trim();
                                var afterDegree = cleaned.Substring(degreeMatch.Index).Trim();
                                
                                if (!string.IsNullOrWhiteSpace(beforeDegree))
                                {
                                    cleaned = $"{beforeDegree} {afterDegree}";
                                }
                            }
                        }

                        education.Add(cleaned);
                    }
                }
            }

            return education;
        }

        private List<string> ExtractProjects(string resumeText)
        {
            var projects = new List<string>();
            var textUpper = resumeText.ToUpper();
            
            var projectsIndex = textUpper.IndexOf("PROJECTS");
            if (projectsIndex == -1)
                return projects;

            // Get text until next section
            var nextSectionIndex = FindNextSectionIndex(textUpper, projectsIndex + 1);
            var projectsSection = resumeText.Substring(projectsIndex,
                nextSectionIndex == -1 ? resumeText.Length - projectsIndex : nextSectionIndex - projectsIndex);

            // Normalize the section first to handle compressed text
            projectsSection = NormalizeResumeText(projectsSection);

            // First try splitting by pipe (|) if it exists
            if (projectsSection.Contains("|"))
            {
                var pipeParts = projectsSection.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in pipeParts)
                {
                    var cleaned = part.Trim();

                    if (cleaned.Equals("PROJECTS", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(cleaned))
                        continue;

                    // Extract only the first part (project name)
                    var projectName = cleaned.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim() ?? "";
                    
                    // Remove numbers and extra text
                    projectName = System.Text.RegularExpressions.Regex.Replace(projectName, @"\d+", "").Trim();
                    projectName = System.Text.RegularExpressions.Regex.Replace(projectName, @"^[-•*\s]+", "").Trim();

                    if (!string.IsNullOrWhiteSpace(projectName) && projectName.Length < 100)
                    {
                        var wordCount = projectName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                        
                        if (wordCount >= 1 && wordCount <= 6)
                        {
                            projects.Add(projectName);
                        }
                    }
                }
            }
            else
            {
                // Fall back to line-based extraction
                var lines = projectsSection.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines)
                {
                    var cleaned = line.Trim()
                        .Replace("•", "")
                        .Replace("*", "")
                        .Trim();

                    if (cleaned.Equals("PROJECTS", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(cleaned))
                        continue;

                    // Get first part before colon or dash
                    var projectName = cleaned;
                    var colonIndex = cleaned.IndexOf(':');
                    var dashIndex = cleaned.IndexOf('-');
                    
                    if (colonIndex > 0 || dashIndex > 0)
                    {
                        var splitIndex = colonIndex > 0 && dashIndex > 0 ? Math.Min(colonIndex, dashIndex) : 
                                       colonIndex > 0 ? colonIndex : dashIndex;
                        projectName = cleaned.Substring(0, splitIndex).Trim();
                    }

                    // Remove numbers
                    projectName = System.Text.RegularExpressions.Regex.Replace(projectName, @"\d+", "").Trim();

                    if (projectName.Length > 0 && projectName.Length < 100)
                    {
                        var wordCount = projectName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                        
                        if (wordCount >= 1 && wordCount <= 6)
                        {
                            projects.Add(projectName);
                        }
                    }
                }
            }

            return projects;
        }

        private List<string> ExtractCertifications(string resumeText)
        {
            var certifications = new List<string>();
            var textUpper = resumeText.ToUpper();
            
            var certIndex = textUpper.IndexOf("CERTIFICATIONS");
            if (certIndex == -1)
                return new() { "No certifications available" };

            // Get text until next section
            var nextSectionIndex = FindNextSectionIndex(textUpper, certIndex + 1);
            var certSection = resumeText.Substring(certIndex,
                nextSectionIndex == -1 ? resumeText.Length - certIndex : nextSectionIndex - certIndex);

            var lines = certSection.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                var cleaned = line.Trim()
                    .Replace("•", "")
                    .Replace("-", "")
                    .Replace("*", "")
                    .Trim();

                if (cleaned.Equals("CERTIFICATIONS", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(cleaned))
                    continue;

                // Look for certification names (not achievements/descriptions)
                if (!cleaned.Contains("achieved") && !cleaned.Contains("accomplished") && 
                    !cleaned.Contains("completed") && cleaned.Length > 3)
                {
                    certifications.Add(cleaned);
                }
            }

            return certifications.Count > 0 ? certifications : new() { "No certifications available" };
        }

        private int FindNextSectionIndex(string textUpper, int startIndex)
        {
            var sections = new[] { "EDUCATION", "PROJECTS", "SKILLS", "TECHNICAL SKILLS", "EXPERIENCE", "WORK EXPERIENCE", "CERTIFICATIONS", "ACHIEVEMENTS", "SUMMARY" };
            int closestIndex = -1;

            foreach (var section in sections)
            {
                var index = textUpper.IndexOf(section, startIndex);
                if (index != -1 && (closestIndex == -1 || index < closestIndex))
                {
                    closestIndex = index;
                }
            }

            return closestIndex;
        }

        private bool ContainsAny(string text, string[] keywords)
        {
            if (string.IsNullOrEmpty(text)) return false;
            var textLower = text.ToLower();
            return keywords.Any(keyword => textLower.Contains(keyword.ToLower()));
        }
    }
}
