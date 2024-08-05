using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    public Task<IdentityResult> CreateAsync(User entity, string password, CancellationToken cancellationToken = default);
    public Task<IdentityResult> AssignRoleAsync(User entity, string role, CancellationToken cancellationToken = default);
    public Task<bool> IsInRoleAsync(User entity, string role, CancellationToken cancellationToken = default);
}