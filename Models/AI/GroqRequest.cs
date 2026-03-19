namespace SkillBridge.Models.AI
{
    public class GroqRequest
    {
        public string model { get; set; } = "llama-3.1-8b-instant";
        public List<Message> messages { get; set; } = new();
        public double temperature { get; set; } = 0.7;
    }

    public class Message
    {
        public string role { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
    }
}
