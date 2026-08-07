using System.ComponentModel.DataAnnotations;

namespace PollSurveyBuilder.API.DTOs
{
    public class VoteDto
    {
        [Required]
        public int OptionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string VoterToken { get; set; } = string.Empty;
    }
}