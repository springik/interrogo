namespace Core.Entities;

public class Question
{
    public string Id { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string Author  { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int FinalScore { get; set; } = 0;
    public bool IsAnswered { get; set; } = false;
}