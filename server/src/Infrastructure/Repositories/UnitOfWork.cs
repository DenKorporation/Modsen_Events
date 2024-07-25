using Domain.Errors;
using Domain.Repositories;
using FluentResults;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repositories;

public class UnitOfWork(
    IEventRepository eventRepository,
    IUserRepository userRepository,
    IEventUserRepository eventUserRepository,
    AppDbContext dbContext)
    : IUnitOfWork
{
    public IEventRepository EventRepository { get; init; } = eventRepository;
    public IUserRepository UserRepository { get; init; } = userRepository;
    public IEventUserRepository EventUserRepository { get; init; } = eventUserRepository;

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" } pex)
        {
            return pex.ConstraintName?[..2] switch
            {
                "PK" => new PrimaryKeyError("duplicate value for primary key", pex.TableName!),
                "IX" => new UniquenessError($"duplicate value for the field {pex.ColumnName}", pex.TableName!,
                    pex.ColumnName!),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23503" } pex)
        {
            var foreignTable = pex.ConstraintName!.Split('_')[2];
            return new ForeignKeyError($"Value is not presented in table {foreignTable}", pex.TableName!, foreignTable);
        }
        catch (Exception ex)
        {
            return Result.Fail(new ExceptionalError(ex));
        }

        return Result.Ok();
    }
}