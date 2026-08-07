using System.ComponentModel.DataAnnotations;

namespace PollSurveyBuilder.API.DTOs
{
    public class CreatePollDto
    {
        [Required]
        [MaxLength(300)]
        public string Question { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        [MaxLength(6)]
        public List<string> Options { get; set; } = new();

        public DateTime? ExpiresAt { get; set; }
    }
}