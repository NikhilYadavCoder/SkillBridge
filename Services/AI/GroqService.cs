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
                    return GetFallbackData();
                }

                var prompt = $@"You are a strict resume parser API.

Return ONLY valid JSON.
Do NOT add explanation.
Do NOT change structure.

------------------------
REQUIRED OUTPUT FORMAT
------------------------

{{
  ""skills"": [],
  ""experience"": [],
  ""education"": [],
  ""projects"": [],
  ""certifications"": []
}}

------------------------
STRICT RULES
------------------------

SKILLS:
- Return ONLY array of strings
- NOT objects
- Example: [""C++"", ""React"", ""ASP.NET""]
- Include:
  programming, web, backend, database, devops tools
- Max 2 words per skill
- Remove:
  platforms (LeetCode), sentences, random phrases

------------------------

EXPERIENCE:
- Extract job roles + company if available
- Example:
  [""Software Engineering Intern - Nalashaa Solutions""]
- If none:
  [""No experience available""]

------------------------

EDUCATION:
- Format:
  ""Degree - Institute Name""
- Example:
  [""B.Tech Computer Science - Indian Institute of Information Technology Kottayam""]
- Remove years, CGPA

------------------------

PROJECTS:
- Only project names
- Example:
  [""Byte Bench"", ""Storistiq"", ""Crack Detection System""]

------------------------

CERTIFICATIONS:
- Only real certifications
- DO NOT include achievements (LeetCode, CodeChef)
- If none:
  [""No certifications available""]

------------------------

VERY IMPORTANT:
- skills must be string array (NOT objects)
- no null values
- no extra fields like fullName, email
- output must match EXACT structure

------------------------

Resume:
{resumeText}";

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
                    return GetFallbackData();
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var groqResponse = JsonSerializer.Deserialize<GroqResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (groqResponse?.choices != null && groqResponse.choices.Count > 0)
                {
                    var jsonResponse = groqResponse.choices[0].message.content;

                    // Clean the AI response before deserialization
                    var cleanedJson = jsonResponse.Trim();
                    if (cleanedJson.StartsWith("```json"))
                    {
                        cleanedJson = cleanedJson.Substring(7);
                    }
                    if (cleanedJson.EndsWith("```"))
                    {
                        cleanedJson = cleanedJson.Substring(0, cleanedJson.Length - 3);
                    }
                    cleanedJson = cleanedJson.Trim();

                    var firstBrace = cleanedJson.IndexOf('{');
                    var lastBrace = cleanedJson.LastIndexOf('}');
                    if (firstBrace != -1 && lastBrace != -1 && lastBrace > firstBrace)
                    {
                        cleanedJson = cleanedJson.Substring(firstBrace, lastBrace - firstBrace + 1);
                    }

                    var parsedData = JsonSerializer.Deserialize<ResumeParsedDto>(cleanedJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return parsedData ?? GetFallbackData();
                }

                return GetFallbackData();
            }
            catch
            {
                return GetFallbackData();
            }
        }

        private ResumeParsedDto GetFallbackData()
        {
            return new ResumeParsedDto
            {
                Skills = new(),
                Experience = new() { new ExperienceDto { JobTitle = "No experience available" } },
                Education = new(),
                Projects = new(),
                Certifications = new() { new SkillDto { Name = "No certifications available" } }
            };
        }
    }
}
