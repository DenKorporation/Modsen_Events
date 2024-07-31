using Application.UseCases.Events.Queries.GetAllUserEvents;
using Application.UseCases.Users.Commands.RegisterUserToEvent;
using Application.UseCases.Users.Commands.UnregisterUserFromEvent;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/users")]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpGet("{userId:guid}/events")]
    public async Task<IResult> GetAllUserEventsAsync(Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetAllUserEventsQuery(userId, pageNumber, pageSize), cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpPost("{userId:guid}/event/{eventId:guid}")]
    public async Task<IResult> RegisterUserToEventAsync(Guid userId, Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new RegisterUserToEventCommand(userId, eventId), cancellationToken);

        return result.ToAspResult(Results.Created);
    }

    [HttpDelete("{userId:guid}/event/{eventId:guid}")]
    public async Task<IResult> UnregisterUserFromEventAsync(Guid userId, Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new UnregisterUserFromEventCommand(userId, eventId), cancellationToken);

        return result.ToAspResult(Results.NoContent);
    }
}