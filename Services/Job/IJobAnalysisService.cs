using SkillBridge.DTOs.Job;
using SkillBridge.DTOs.Resume;

namespace SkillBridge.Services.Job
{
    public interface IJobAnalysisService
    {
        Task<JobAnalysisResponseDto> AnalyzeAsync(
            List<string> userSkills,
            string role,
            int pageNumber,
            int pageSize);

        Task<JobAnalysisResponseDto> AnalyzeFromResumeAsync(
            ResumeParsedDto resumeData,
            string resumeText,
            string role,
            int pageNumber,
            int pageSize);

        Task<List<string>> GetAvailableRolesAsync();
    }
}
