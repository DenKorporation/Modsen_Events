using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EventUserRepository(AppDbContext dbContext) : IEventUserRepository
{
    public IQueryable<User> GetAllUsersFromEvent(Guid eventId)
    {
        return dbContext.EventUsers
            .Where(eu => eu.EventId == eventId)
            .Select(eu => eu.User);
    }

    public IQueryable<Event> GetAllEventsFromUser(Guid userId)
    {
        return dbContext.EventUsers
            .Where(eu => eu.UserId == userId)
            .Select(eu => eu.Event);
    }

    public async Task AddUserToEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
    {
        await dbContext.EventUsers
            .AddAsync(new EventUser
            {
                UserId = userId,
                EventId = eventId,
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
            }, cancellationToken);
    }

    public async Task<bool> RemoveUserFromEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var eventUser = await dbContext.EventUsers
            .FirstOrDefaultAsync(eu => eu.UserId == userId && eu.EventId == eventId, cancellationToken);

        if (eventUser is null)
        {
            return false;
        }

        dbContext.EventUsers.Remove(eventUser);
        
        return true;
    }
}