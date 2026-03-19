using SkillBridge.DTOs.Resume;

namespace SkillBridge.Services.AI
{
    public interface IGroqService
    {
        Task<ResumeParsedDto> ExtractResumeDataAsync(string resumeText);
        Task<string> GenerateAsync(string prompt);
    }
}
