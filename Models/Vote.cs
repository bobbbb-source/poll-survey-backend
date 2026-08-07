using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PollSurveyBuilder.API.Models
{
    public class Vote
    {
        public int Id { get; set; }

        public int PollId { get; set; }

        [JsonIgnore]
        public Poll Poll { get; set; } = null!;

        public int PollOptionId { get; set; }

        [JsonIgnore]
        public PollOption PollOption { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string VoterToken { get; set; } = string.Empty;

        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    }
}