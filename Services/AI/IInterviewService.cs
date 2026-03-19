using SkillBridge.DTOs.Job;

namespace SkillBridge.Services.AI
{
    public interface IInterviewService
    {
        Task<InterviewResponseDto> GenerateQuestionsAsync(List<string> skills);
    }
}
