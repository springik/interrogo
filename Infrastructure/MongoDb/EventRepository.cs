using Core.Entities;
using Core.Interfaces;
using MongoDB.Driver;

namespace Infrastructure.MongoDb;

public class EventRepository : IEventRepository
{
    private readonly IMongoCollection<Event> _collection;

    public EventRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Event>("Events");
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _collection.Find(_ => true)
            .ToListAsync();
    }
    
    public async Task<Event?> GetIdAsync(string id)
    {
        return await _collection.Find(e => e.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Event?> GetByCodeAsync(string eventCode)
    {
        return await _collection.Find(e => e.EventCode == eventCode)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Event newEvent)
    {
        await _collection.InsertOneAsync(newEvent);
    }

    public async Task UpdateAsync(Event updatedEvent)
    {
        await _collection.ReplaceOneAsync(e => e.Id == updatedEvent.Id, updatedEvent);
    }
}