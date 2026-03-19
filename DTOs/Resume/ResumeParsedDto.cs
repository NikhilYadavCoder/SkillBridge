namespace SkillBridge.DTOs.Resume
{
    public class ResumeParsedDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Summary { get; set; }
        public List<SkillDto> Skills { get; set; } = new();
        public List<ExperienceDto> Experience { get; set; } = new();
        public List<EducationDto> Education { get; set; } = new();
        public List<ProjectDto> Projects { get; set; } = new();
        public List<SkillDto> Certifications { get; set; } = new();
    }

    public class SkillDto
    {
        public string Name { get; set; } = string.Empty;
        public int? ProficiencyLevel { get; set; }
    }

    public class ExperienceDto
    {
        public string? JobTitle { get; set; }
        public string? Company { get; set; }
        public string? Duration { get; set; }
        public string? Description { get; set; }
    }

    public class EducationDto
    {
        public string? Degree { get; set; }
        public string? Institution { get; set; }
        public string? GraduationYear { get; set; }
    }

    public class ProjectDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}

