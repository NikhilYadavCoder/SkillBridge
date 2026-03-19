using SkillBridge.DTOs.Job;

namespace SkillBridge.Services.AI
{
    public interface IRoadmapService
    {
        Task<RoadmapResponseDto> GenerateRoadmapAsync(List<string> skills);
    }
}
