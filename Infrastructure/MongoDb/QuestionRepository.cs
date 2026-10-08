using Core.Entities;
using Core.Interfaces;
using MongoDB.Driver;

namespace Infrastructure.MongoDb;

public class QuestionRepository : IQuestionRepository
{
    private readonly IMongoCollection<Question> _collection;

    public QuestionRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Question>("Questions");
    }
    
    public async Task<IEnumerable<Question>> GetByEventIdAsync(string eventId)
    {
        return await _collection
            .Find(q => q.EventId == eventId)
            .SortBy(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task CreateAsync(Question question)
    {
        await _collection.InsertOneAsync(question);
    }

    public async Task UpdateAsync(Question updatedQuestion)
    {
        await _collection.ReplaceOneAsync(q => q.Id == updatedQuestion.Id, updatedQuestion);
    }
}