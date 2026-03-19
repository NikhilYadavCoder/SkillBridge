using SkillBridge.DTOs.Job;

namespace SkillBridge.Services.Job
{
    public interface IJobAnalysisService
    {
        Task<JobAnalysisResponseDto> AnalyzeAsync(
            List<string> userSkills,
            string role,
            int pageNumber,
            int pageSize);
    }
}
