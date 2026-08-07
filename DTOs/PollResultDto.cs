namespace PollSurveyBuilder.API.DTOs
{
    public class PollResultDto
    {
        public string Question { get; set; } = string.Empty;
        public List<OptionResultDto> Results { get; set; } = new();
    }

    public class OptionResultDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Votes { get; set; }
    }
}