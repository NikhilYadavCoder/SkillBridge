using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using SkillBridge.DTOs.Job;
using SkillBridge.Models.AI;

namespace SkillBridge.Services.AI
{
    public class RoadmapService : IRoadmapService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RoadmapService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<RoadmapResponseDto> GenerateRoadmapAsync(List<string> skills)
        {
            // STEP 1: If skills empty → return empty roadmap
            if (skills == null || skills.Count == 0)
            {
                return new RoadmapResponseDto
                {
                    Roadmap = new List<RoadmapItemDto>()
                };
            }

            // Normalize skills list (trim, remove empty, distinct, preserve original case where possible)
            var normalizedSkills = skills
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (normalizedSkills.Count == 0)
            {
                return new RoadmapResponseDto
                {
                    Roadmap = new List<RoadmapItemDto>()
                };
            }

            // STEP 2: AI CALL (Groq) – use Groq:ApiKey if available
            var apiKey = _configuration["Groq:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey) && !string.Equals(apiKey, "your-groq-api-key-here", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var promptBuilder = new StringBuilder();
                    promptBuilder.AppendLine("You are an expert career mentor.");
                    promptBuilder.AppendLine();
                    promptBuilder.AppendLine("Generate a step-by-step learning roadmap for each skill.");
                    promptBuilder.AppendLine();
                    promptBuilder.AppendLine("Include strictly:");
                    promptBuilder.AppendLine("- At least 5-6 concrete steps per skill in logical order (beginner → advanced)");
                    promptBuilder.AppendLine("- Real-world tasks such as building projects, deploying, and integrating with other tools");
                    promptBuilder.AppendLine("- 2-3 specific resources per skill (YouTube playlists, official docs, well-known courses)");
                    promptBuilder.AppendLine("- A realistic estimated time range for job-ready proficiency (for example '1-2 weeks', '2-4 weeks')");
                    promptBuilder.AppendLine();
                    promptBuilder.AppendLine("Return STRICT JSON:");
                    promptBuilder.AppendLine();
                    promptBuilder.AppendLine("{");
                    promptBuilder.AppendLine("  \"roadmap\": [");
                    promptBuilder.AppendLine("    {");
                    promptBuilder.AppendLine("      \"skill\": \"\",");
                    promptBuilder.AppendLine("      \"steps\": [],");
                    promptBuilder.AppendLine("      \"resources\": [],");
                    promptBuilder.AppendLine("      \"estimatedTime\": \"\"");
                    promptBuilder.AppendLine("    }");
                    promptBuilder.AppendLine("  ]");
                    promptBuilder.AppendLine("}");
                    promptBuilder.AppendLine();
                    promptBuilder.Append("Skills: ");
                    promptBuilder.Append(string.Join(", ", normalizedSkills));

                    var prompt = promptBuilder.ToString();

                    var request = new GroqRequest
                    {
                        model = "llama-3.1-8b-instant",
                        messages = new List<Message>
                        {
                            new Message { role = "user", content = prompt }
                        },
                        temperature = 0.3
                    };

                    var requestBody = JsonSerializer.Serialize(request);
                    using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                    var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var groqResponse = JsonSerializer.Deserialize<GroqResponse>(responseString, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (groqResponse?.choices != null && groqResponse.choices.Count > 0)
                        {
                            var jsonResponse = groqResponse.choices[0].message.content ?? string.Empty;

                            var cleanedJson = CleanJson(jsonResponse);

                            if (!string.IsNullOrWhiteSpace(cleanedJson))
                            {
                                var roadmap = JsonSerializer.Deserialize<RoadmapResponseDto>(cleanedJson, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                                if (roadmap != null && roadmap.Roadmap != null)
                                {
                                    return roadmap;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Swallow and fallback below
                }
            }

            // STEP 4 & 5: FALLBACK
            var fallbackRoadmap = BuildFallbackRoadmap(normalizedSkills);

            return new RoadmapResponseDto
            {
                Roadmap = fallbackRoadmap
            };
        }

        private static string CleanJson(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

            var cleaned = raw.Trim();

            // Remove markdown code blocks
            cleaned = Regex.Replace(cleaned, @"```json\s*", string.Empty, RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"```\s*", string.Empty, RegexOptions.IgnoreCase);

            // Extract content between first { and last }
            var firstBrace = cleaned.IndexOf('{');
            var lastBrace = cleaned.LastIndexOf('}');

            if (firstBrace != -1 && lastBrace != -1 && lastBrace > firstBrace)
            {
                cleaned = cleaned.Substring(firstBrace, lastBrace - firstBrace + 1);
            }

            return cleaned.Trim();
        }

        private static List<RoadmapItemDto> BuildFallbackRoadmap(List<string> skills)
        {
            var fallbackMap = new Dictionary<string, RoadmapItemDto>(StringComparer.OrdinalIgnoreCase)
            {
                ["Docker"] = new RoadmapItemDto
                {
                    Skill = "Docker",
                    Steps = new List<string>
                    {
                        "Understand containers vs virtual machines and Docker architecture",
                        "Install Docker and run official hello-world and nginx containers",
                        "Work with images: pull, tag, push to Docker Hub",
                        "Write a Dockerfile for a simple API or web app",
                        "Use docker-compose to run an app with a database locally",
                        "Containerize and run a small portfolio project end-to-end"
                    },
                    Resources = new List<string>
                    {
                        "Official Docker documentation (docs.docker.com)",
                        "Docker 'Get Started' guide",
                        "YouTube: Docker crash course / full playlist"
                    },
                    EstimatedTime = "5-7 days"
                },
                ["JWT"] = new RoadmapItemDto
                {
                    Skill = "JWT",
                    Steps = new List<string>
                    {
                        "Learn JWT structure (header, payload, signature) and common claims",
                        "Implement basic login that issues JWTs in your preferred backend stack",
                        "Protect API routes using JWT validation middleware/filters",
                        "Add refresh tokens and token expiry handling",
                        "Test secured endpoints with tools like Postman or Thunder Client",
                        "Refine security: use strong signing keys, HTTPS, and proper token storage on the client"
                    },
                    Resources = new List<string>
                    {
                        "jwt.io introduction and debugger",
                        "Auth0 JWT docs and blog posts",
                        "YouTube: JWT authentication tutorial for your tech stack"
                    },
                    EstimatedTime = "3-5 days"
                },
                ["AWS"] = new RoadmapItemDto
                {
                    Skill = "AWS",
                    Steps = new List<string>
                    {
                        "Create an AWS account and understand IAM users, roles, and permissions",
                        "Learn EC2 basics and launch a Linux instance",
                        "Deploy a simple web application or API to EC2",
                        "Store and serve static assets from S3 with proper security settings",
                        "Connect an application to a managed database service (e.g., RDS)",
                        "Add monitoring and cost awareness using CloudWatch and the billing dashboard"
                    },
                    Resources = new List<string>
                    {
                        "Official AWS documentation and tutorials",
                        "AWS free training on aws.training",
                        "YouTube: AWS beginner to intermediate playlists"
                    },
                    EstimatedTime = "1-2 weeks"
                }
            };

            var roadmap = new List<RoadmapItemDto>();

            foreach (var skill in skills)
            {
                if (string.IsNullOrWhiteSpace(skill)) continue;

                if (fallbackMap.TryGetValue(skill, out var predefined))
                {
                    // Clone to avoid shared list references and preserve original skill casing
                    roadmap.Add(new RoadmapItemDto
                    {
                        Skill = skill,
                        Steps = new List<string>(predefined.Steps),
                        Resources = new List<string>(predefined.Resources),
                        EstimatedTime = predefined.EstimatedTime
                    });
                }
                else
                {
                    // Generic roadmap aimed at job preparation
                    roadmap.Add(new RoadmapItemDto
                    {
                        Skill = skill,
                        Steps = new List<string>
                        {
                            $"Understand core concepts and terminology of {skill}",
                            $"Follow a structured tutorial or course to cover fundamentals of {skill}",
                            $"Implement 3-5 small practice exercises or mini features using {skill}",
                            $"Build a small end-to-end project that you can add to your portfolio using {skill}",
                            $"Refactor the project applying best practices, testing, and clean code patterns",
                            $"Document the project (README, screenshots) and rehearse explaining design decisions for interviews"
                        },
                        Resources = new List<string>
                        {
                            $"Official documentation / guides for {skill}",
                            $"YouTube playlist or crash course focused on {skill}",
                            $"A well-reviewed free or low-cost course / blog series on {skill}"
                        },
                        EstimatedTime = "1-2 weeks"
                    });
                }
            }

            return roadmap;
        }
    }
}
