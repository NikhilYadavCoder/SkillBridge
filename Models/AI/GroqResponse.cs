namespace SkillBridge.Models.AI
{
    public class GroqResponse
    {
        public List<Choice> choices { get; set; } = new();
    }

    public class Choice
    {
        public ResponseMessage message { get; set; } = new();
    }

    public class ResponseMessage
    {
        public string content { get; set; } = string.Empty;
    }
}
