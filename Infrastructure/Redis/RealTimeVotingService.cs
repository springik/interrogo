using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Redis;

public class RealTimeVotingService : IRealTimeVotingService
{
    private readonly IDatabase _redisDb;

    public RealTimeVotingService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }
    
    private string GetEventKey(string eventId) => $"event:{eventId}:questions";
    private string GetVotersKey(string eventId, string questionId) => $"event:{eventId}:question:{questionId}:voters";

    public async Task<bool> ToggleUpvoteAsync(string eventId, string questionId, string userId)
    {
        var votersKey = GetVotersKey(eventId, questionId);
        
        // Key exists => true
        // key does not exist => false
        var wasAdded = await _redisDb.SetAddAsync(votersKey, userId);
        
        if (wasAdded)
        {
            await _redisDb.SortedSetIncrementAsync(GetEventKey(eventId), questionId, 1);
        }
        else
        {
            await _redisDb.SetRemoveAsync(votersKey, userId);
            await _redisDb.SortedSetDecrementAsync(GetEventKey(eventId), questionId, 1);
        }

        return wasAdded;
    }

    public async Task<bool> HasUserUpvotedAsync(string eventId, string questionId, string userId)
    {
        return await _redisDb.SetContainsAsync(GetVotersKey(eventId, questionId), userId);
    }

    public async Task UpvoteQuestionAsync(string eventId, string questionId)
    {
        await _redisDb.SortedSetIncrementAsync(GetEventKey(eventId), questionId, 1);
    }

    public async Task<int> GetQuestionScoreAsync(string eventId, string questionId)
    {
        var score = await _redisDb.SortedSetScoreAsync(GetEventKey(eventId), questionId);
        return score.HasValue ? (int)score.Value : 0;
    }

    public async Task<IEnumerable<string>> GetTopQuestionsAsync(string eventId, int limit = 50)
    {
        var topQuestions = await _redisDb.SortedSetRangeByRankAsync(
            GetEventKey(eventId),
            start: 0,
            stop: limit - 1,
            order: Order.Descending
        );

        return topQuestions.Select(q => q.ToString());
    }

    public async Task ClearEventDataAsync(string eventId)
    {
        await _redisDb.KeyDeleteAsync(GetEventKey(eventId));
    }
}