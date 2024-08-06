using Application.UseCases.Events.Queries.GetAllUserEvents;
using Application.UseCases.Users.Commands.AssignRoleToUser;
using Application.UseCases.Users.Commands.CreateUser;
using Application.UseCases.Users.Commands.DeleteUser;
using Application.UseCases.Users.Commands.RegisterUserToEvent;
using Application.UseCases.Users.Commands.UnregisterUserFromEvent;
using Application.UseCases.Users.Queries.GetAllUsers;
using Application.UseCases.Users.Queries.GetUserById;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers;

[ApiController]
[Authorize(Policies.ApiScope)]
[Route("/api/users")]
public class UsersController(ISender sender) : ControllerBase
{
    [Authorize(Policies.AdminRole)]
    [HttpGet]
    public async Task<IResult> GetAllUsersAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetAllUsersQuery(pageNumber, pageSize), cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [Authorize(Policies.UserIdMatchingOrAdminRole)]
    [HttpGet("{userId:guid}/events")]
    public async Task<IResult> GetAllUserEventsAsync(Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetAllUserEventsQuery(userId, pageNumber, pageSize), cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [Authorize(Policies.UserIdMatchingOrAdminRole)]
    [HttpGet("{userId:guid}")]
    public async Task<IResult> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetUserByIdQuery(userId), cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IResult> CreateUserAsync([FromBody] CreateUserCommand createUserCommand,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(createUserCommand, cancellationToken);

        return result.ToAspResult(value => Results.Created(string.Empty, value));
    }

    [Authorize(Policies.UserIdMatchingOrAdminRole)]
    [HttpPost("{userId:guid}/event/{eventId:guid}")]
    public async Task<IResult> RegisterUserToEventAsync(Guid userId, Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new RegisterUserToEventCommand(userId, eventId), cancellationToken);

        return result.ToAspResult(Results.Created);
    }

    [Authorize(Policies.UserIdMatchingOrAdminRole)]
    [HttpDelete("{userId:guid}/event/{eventId:guid}")]
    public async Task<IResult> UnregisterUserFromEventAsync(Guid userId, Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new UnregisterUserFromEventCommand(userId, eventId), cancellationToken);

        return result.ToAspResult(Results.NoContent);
    }

    [Authorize(Policies.AdminRole)]
    [HttpPut("{userId:guid}")]
    public async Task<IResult> AssignRoleToUserAsync(Guid userId, string role,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new AssignRoleToUserCommand(userId, role), cancellationToken);

        return result.ToAspResult(Results.Ok());
    }

    [Authorize(Policies.UserIdMatchingOrAdminRole)]
    [HttpDelete("{userId:guid}")]
    public async Task<IResult> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DeleteUserCommand(userId), cancellationToken);

        return result.ToAspResult(Results.NoContent);
    }
}