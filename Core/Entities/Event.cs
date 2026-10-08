namespace Core.Entities;

public class Event
{
    public string Id { get; set; } = string.Empty;
    public string EventCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}