using Application.Common.Extensions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories;

public class UserRepository(AppDbContext dbContext, UserManager<User> userManager)
    : Repository<User>(dbContext), IUserRepository
{
    public async Task<IdentityResult> CreateAsync(User entity, string password,
        CancellationToken cancellationToken = default)
    {
        var result = await userManager.CreateAsync(entity, password);
        if (!result.Succeeded)
        {
            return result;
        }

        return await userManager.AddClaimsAsync(entity, entity.ToClaims());
    }

    public async Task<IdentityResult> AssignRoleAsync(User entity, string role,
        CancellationToken cancellationToken = default)
    {
        var result = await RemoveAllUserRoleAsync(entity, cancellationToken);
        if (!result.Succeeded)
        {
            return result;
        }

        return await userManager.AddToRoleAsync(entity, role);
    }

    public async Task<bool> IsInRoleAsync(User entity, string role, CancellationToken cancellationToken = default)
    {
        return await userManager.IsInRoleAsync(entity, role);
    }

    private async Task<IdentityResult> RemoveAllUserRoleAsync(User user, CancellationToken cancellationToken = default)
    {
        return await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
    }
}