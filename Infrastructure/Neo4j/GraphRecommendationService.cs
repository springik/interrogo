using Core.Entities;
using Core.Interfaces;
using Neo4j.Driver;

namespace Infrastructure.Neo4j;

public class GraphRecommendationService : IGraphRecommendationService
{
    private readonly IDriver _driver;

    public GraphRecommendationService(IDriver driver)
    {
        _driver = driver;
    }
    
    public async Task RegisterUserUpvoteAsync(string userId, string questionId)
    {
        var query = @"
            MERGE (u:User {id: $userId})
            MERGE (q:Question {id: $questionId})
            MERGE (u)-[:UPVOTED]->(q)";
        
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            await tx.RunAsync(query, new { userId, questionId });
        });
    }

    public async Task RegisterQuestionTagAsync(string questionId, string tag)
    {
        var query = @"
            MERGE (q:Question {id: $questionId})
            MERGE (t:Tag {name: $tag})
            MERGE (q)-[:HAS_TAG]->(t)";
        
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            await tx.RunAsync(query, new { questionId, tag });
        });
    }

    public async Task<IEnumerable<UserRecommendation>> GetSimilarUsersAsync(string userId, int limit = 5)
    {
        var query = @"
            MATCH (me:User {id: $userId})-[:UPVOTED]->(:Question)-[:HAS_TAG]->(t:Tag)
            MATCH (t)<-[:HAS_TAG]-(:Question)<-[:UPVOTED]-(otherUser:User)
            WHERE me <> otherUser
            RETURN otherUser.id as UserId, count(t) as SharedTagsCount
            ORDER BY SharedTagCount DESC
            LIMIT $limit";
        
        await using var session = _driver.AsyncSession();
        var result = await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, new { userId, limit });
            return await cursor.ToListAsync(record => new UserRecommendation
            {
                UserId = record["UserId"].As<string>(),
                SharedTagsCount = record["SharedTagsCount"].As<int>()
            });
        });
        
        return result;
    }
}