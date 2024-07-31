using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EventRepository(AppDbContext dbContext) : Repository<Event>(dbContext), IEventRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<Event?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Name.ToLower() == name.ToLower(), cancellationToken);
    }
}