using System.ComponentModel.DataAnnotations;

namespace PollSurveyBuilder.API.Models
{
    public class Poll
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Question { get; set; } = string.Empty;

        public bool IsClosed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiresAt { get; set; }

        [Required]
        [MaxLength(100)]
        public string CreatorToken { get; set; } = string.Empty;

        public ICollection<PollOption> Options { get; set; }
            = new List<PollOption>();

        public ICollection<Vote> Votes { get; set; }
            = new List<Vote>();
    }
}