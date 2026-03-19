namespace SkillBridge.DTOs.Job
{
    public class InterviewQuestionDto
    {
        public string Skill { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty; // Easy, Medium, Hard
    }

    public class InterviewResponseDto
    {
        public List<InterviewQuestionDto> Questions { get; set; } = new();
    }
}
