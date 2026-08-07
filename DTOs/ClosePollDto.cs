using System.ComponentModel.DataAnnotations;

namespace PollSurveyBuilder.API.DTOs
{
    public class ClosePollDto
    {
        [Required]
        public string CreatorToken { get; set; } = string.Empty;
    }
}