using Core.Entities;

namespace Core.Interfaces;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetAllAsync();
    Task<Event?> GetIdAsync(string id);
    Task<Event?> GetByCodeAsync(string eventCode);
    Task CreateAsync(Event newEvent);
    Task UpdateAsync(Event updatedEvent);
}