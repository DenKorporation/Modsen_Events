using Domain.Entities;

namespace Domain.Repositories;

public interface IEventRepository : IRepository<Event>
{
    public Task<Event?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}