using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.Common.Interfaces.Messaging;
using Domain.Entities;
using Domain.Repositories;
using FluentResults;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Users.Commands.AssignRoleToUser;

public class AssignRoleToUserHandler(IUnitOfWork unitOfWork, UserManager<User> userManager)
    : ICommandHandler<AssignRoleToUserCommand>
{
    public async Task<Result> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var dbUser = await userManager.FindByIdAsync(request.UserId.ToString());

        if (dbUser is null)
        {
            return new UserNotFoundError(message: $"User '{request.UserId}' not found");
        }

        if (await unitOfWork.UserRepository.IsInRoleAsync(dbUser, request.Role, cancellationToken))
        {
            return new ConflictError("UserRole.Update", $"User {request.UserId} already has a role {request.Role}");
        }

        var roleResult = await unitOfWork.UserRepository.AssignRoleAsync(dbUser, request.Role, cancellationToken);

        if (!roleResult.Succeeded)
        {
            return new InternalServerError("UserRole.Update", "Something went wrong when saving the data");
        }

        dbUser.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.UserRepository.UpdateAsync(dbUser, cancellationToken);

        await UpdateUpdatedAtClaimAsync(dbUser);

        var dbResult = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (dbResult.IsFailed)
        {
            return new InternalServerError("UserRole.Update", "Something went wrong when saving the data");
        }

        return Result.Ok();
    }

    private async Task UpdateUpdatedAtClaimAsync(User user)
    {
        var updateAtClaim =
            (await userManager.GetClaimsAsync(user)).First(c => c.Type == JwtClaimTypes.UpdatedAt);

        await userManager.ReplaceClaimAsync(user, updateAtClaim,
            new Claim(JwtClaimTypes.UpdatedAt, new DateTimeOffset(user.UpdatedAt).ToUnixTimeSeconds().ToString()));
    }
}