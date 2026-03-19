using System.Text.Json;
using SkillBridge.DTOs.Job;

namespace SkillBridge.Services.AI
{
    public class InterviewService : IInterviewService
    {
        private readonly IGroqService _groqService;

        public InterviewService(IGroqService groqService)
        {
            _groqService = groqService;
        }

        public async Task<InterviewResponseDto> GenerateQuestionsAsync(List<string> skills)
        {
            if (skills == null || !skills.Any())
                return new InterviewResponseDto { Questions = new List<InterviewQuestionDto>() };

                        try
                        {
                                var skillsList = string.Join(", ", skills);

                                var prompt = $$"""
You are a technical interviewer.

Generate interview questions based on the following skills.

Include:
- mix of Easy, Medium, Hard
- practical and conceptual questions

Return STRICT JSON:

{
    "questions": [
        {
            "skill": "",
            "question": "",
            "difficulty": "Easy"
        }
    ]
}

Skills: {{skillsList}}
""";

                                var aiResponse = await _groqService.GenerateAsync(prompt);

                var result = JsonSerializer.Deserialize<InterviewResponseDto>(
                    aiResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result != null && result.Questions?.Any() == true)
                    return result;
            }
            catch
            {
                // fallback
            }

            var fallback = new List<InterviewQuestionDto>();

            foreach (var skill in skills)
            {
                fallback.Add(new InterviewQuestionDto
                {
                    Skill = skill,
                    Question = $"What is {skill} and where is it used?",
                    Difficulty = "Easy"
                });

                fallback.Add(new InterviewQuestionDto
                {
                    Skill = skill,
                    Question = $"Explain how you have used {skill} in a project.",
                    Difficulty = "Medium"
                });

                fallback.Add(new InterviewQuestionDto
                {
                    Skill = skill,
                    Question = $"What are advanced concepts or challenges in {skill}?",
                    Difficulty = "Hard"
                });
            }

            return new InterviewResponseDto { Questions = fallback };
        }
    }
}
