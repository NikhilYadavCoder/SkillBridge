using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkillBridge.DTOs.Profile
{
    public class UpdateSkillsDto
    {
        [Required(ErrorMessage = "Skills are required")]
        [JsonPropertyName("skills")]
        public List<string> Skills { get; set; } = new List<string>();
    }
}
