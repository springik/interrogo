using Core.Entities;

namespace Core.Interfaces;

public interface IRealTimeVotingService
{
    Task<bool> ToggleUpvoteAsync(string eventId, string questionId, string userId);
    Task<bool> HasUserUpvotedAsync(string eventId, string questionId, string userId);
    Task UpvoteQuestionAsync(string eventId, string questionId);
    Task<int> GetQuestionScoreAsync(string eventId, string questionId);
    Task<IEnumerable<string>> GetTopQuestionsAsync(string eventId, int limit = 50);
    Task ClearEventDataAsync(string eventId);
}