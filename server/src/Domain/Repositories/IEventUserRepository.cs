using Domain.Entities;

namespace Domain.Repositories;

public interface IEventUserRepository
{
    public IQueryable<User> GetAllUsersFromEvent(Guid eventId);
    public IQueryable<Event> GetAllEventsFromUser(Guid userId);
    public Task AddUserToEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default);
    public Task<bool> RemoveUserFromEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default);
}