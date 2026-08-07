using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PollSurveyBuilder.API.Models
{
    public class PollOption
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Text { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public int PollId { get; set; }

        [JsonIgnore]
        public Poll Poll { get; set; } = null!;

        public ICollection<Vote> Votes { get; set; }
            = new List<Vote>();
    }
}