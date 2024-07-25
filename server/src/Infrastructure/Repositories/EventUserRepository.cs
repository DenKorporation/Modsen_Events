using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EventUserRepository(AppDbContext dbContext) : Repository<EventUser>(dbContext), IEventUserRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public IQueryable<User> GetAllUsersFromEvent(Guid eventId)
    {
        return _dbContext.EventUsers
            .Where(eu => eu.EventId == eventId)
            .Select(eu => eu.User);
    }

    public IQueryable<Event> GetAllEventsFromUser(Guid userId)
    {
        return _dbContext.EventUsers
            .Where(eu => eu.UserId == userId)
            .Select(eu => eu.Event);
    }

    public async Task AddUserToEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
    {
        await _dbContext.EventUsers
            .AddAsync(new EventUser
            {
                UserId = userId,
                EventId = eventId,
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
            }, cancellationToken);
    }

    public async Task<bool> RemoveUserFromEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var eventUser = await _dbContext.EventUsers
            .FirstOrDefaultAsync(eu => eu.UserId == userId && eu.EventId == eventId, cancellationToken);

        if (eventUser is null)
        {
            return false;
        }

        await DeleteAsync(eventUser, cancellationToken);
        
        return true;
    }
}