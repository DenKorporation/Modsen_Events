using FluentResults;

namespace Domain.Repositories;

public interface IUnitOfWork
{
    public IEventRepository EventRepository { get; init; }
    public IUserRepository UserRepository { get; init; }
    public IEventUserRepository EventUserRepository { get; init; }
    
    public Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}