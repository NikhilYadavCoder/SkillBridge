namespace SkillBridge.DTOs.Job
{
    public class RoadmapItemDto
    {
        public string Skill { get; set; } = string.Empty;
        public List<string> Steps { get; set; } = new();
        public List<string> Resources { get; set; } = new();
        public string EstimatedTime { get; set; } = string.Empty;
    }

    public class RoadmapResponseDto
    {
        public List<RoadmapItemDto> Roadmap { get; set; } = new();
    }
}
