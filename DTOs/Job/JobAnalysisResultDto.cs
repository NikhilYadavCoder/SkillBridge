namespace SkillBridge.DTOs.Job
{
    public class JobVariantResultDto
    {
        public string Variant { get; set; } = string.Empty;
        public List<string> MatchedSkills { get; set; } = new();
        public List<string> MissingSkills { get; set; } = new();
        public double MatchPercentage { get; set; }
    }

    public class JobAnalysisResponseDto
    {
        public string Role { get; set; } = string.Empty;
        public List<JobVariantResultDto> Results { get; set; } = new();
        public List<string> AggregatedMissingSkills { get; set; } = new();
    }
}
