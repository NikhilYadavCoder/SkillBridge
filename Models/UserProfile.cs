namespace SkillBridge.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? Skills { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public User? User { get; set; }
    }
}
