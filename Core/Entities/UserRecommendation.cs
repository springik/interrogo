namespace Core.Entities;

public class UserRecommendation
{
    public string UserId { get; set; } = String.Empty;
    public int SharedTagsCount { get; set; }
}