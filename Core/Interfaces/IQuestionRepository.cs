using Core.Entities;

namespace Core.Interfaces;

public interface IQuestionRepository
{
    Task<IEnumerable<Question>> GetByEventIdAsync (string eventId);
    Task CreateAsync(Question question);
    Task UpdateAsync(Question updatedQuestion);
}