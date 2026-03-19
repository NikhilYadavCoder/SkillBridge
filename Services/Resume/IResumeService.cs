using SkillBridge.DTOs.Resume;

namespace SkillBridge.Services.Resume
{
    public interface IResumeService
    {
        Task<string> ExtractTextFromPdfAsync(IFormFile file);
        Task<ResumeExtractionResultDto> ExtractAndParseResumeAsync(IFormFile file);
        Task<object> ProcessResumeAsync(IFormFile file);
    }
}
