using Core.Entities;

namespace Core.Interfaces;

public interface IGraphRecommendationService
{
    Task RegisterUserUpvoteAsync(string userId, string questionId);
    Task RegisterQuestionTagAsync(string questionId, string tag);
    Task<IEnumerable<UserRecommendation>> GetSimilarUsersAsync(string userId, int limit = 5);
}