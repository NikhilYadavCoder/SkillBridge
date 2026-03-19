namespace SkillBridge.DTOs.Resume
{
    public class ResumeParsedDto
    {
        public List<string> skills { get; set; } = new();
        public List<string> experience { get; set; } = new();
        public List<string> education { get; set; } = new();
        public List<string> projects { get; set; } = new();
        public List<string> certifications { get; set; } = new();
    }
}

