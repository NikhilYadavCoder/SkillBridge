namespace SkillBridge.DTOs.Resume
{
    public class ResumeExtractionResultDto
    {
        public string? ExtractedText { get; set; }
        public ResumeParsedDto? StructuredData { get; set; }
    }
}
