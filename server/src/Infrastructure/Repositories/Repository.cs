using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class Repository<TEntity>(AppDbContext dbContext) : IRepository<TEntity> where TEntity : class
{
    public Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(dbContext
            .Set<TEntity>()
            .AsNoTracking()
            .AsQueryable());
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext
            .Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await dbContext
            .Set<TEntity>()
            .AddAsync(entity, cancellationToken);

        return entity;
    }

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        dbContext
            .Set<TEntity>()
            .Update(entity);

        return Task.FromResult(entity);
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        dbContext
            .Set<TEntity>()
            .Remove(entity);

        return Task.CompletedTask;
    }

    public Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext
            .Set<TEntity>()
            .AnyAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
    }
}