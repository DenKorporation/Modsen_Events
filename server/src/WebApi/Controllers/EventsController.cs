using Application.UseCases.Events.Commands.CreateEvent;
using Application.UseCases.Events.Commands.DeleteEvent;
using Application.UseCases.Events.Commands.DeleteImage;
using Application.UseCases.Events.Commands.UpdateEvent;
using Application.UseCases.Events.Commands.UpdateImage;
using Application.UseCases.Events.Commands.UploadImage;
using Application.UseCases.Events.Queries.GetEventById;
using Application.UseCases.Events.Queries.GetEventByName;
using Application.UseCases.Events.Queries.GetFilteredEvents;
using Application.UseCases.Users.Queries.GetAllUsersFromEvent;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/events")]
public class EventsController(ISender sender) : ControllerBase
{
    [HttpGet("{eventId:guid}")]
    public async Task<IResult> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetEventByIdQuery(eventId), cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpGet("{name}")]
    public async Task<IResult> GetEventByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetEventByNameQuery(name), cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpGet("{eventId:guid}/users")]
    public async Task<IResult> GetAllUsersFromEventAsync(Guid eventId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetAllUsersFromEventQuery(eventId, pageNumber, pageSize), cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpGet]
    public async Task<IResult> GetFilteredEventsAsync([FromQuery] GetFilteredEventsQuery filteredEventsQuery,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(filteredEventsQuery, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpPost]
    public async Task<IResult> CreateEventAsync([FromBody] CreateEventCommand createEventCommand,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(createEventCommand, cancellationToken);

        return result.ToAspResult(value => Results.Created(string.Empty, value));
    }

    [HttpPut]
    public async Task<IResult> UpdateEventAsync([FromBody] UpdateEventCommand updateEventCommand,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(updateEventCommand, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpDelete("{eventId:guid}")]
    public async Task<IResult> DeleteEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DeleteEventCommand(eventId), cancellationToken);

        return result.ToAspResult(Results.NoContent);
    }

    [HttpPost("{eventId:guid}/preview")]
    public async Task<IResult> UploadImageAsync([FromForm] IFormFile previewImage, Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new UploadImageCommand(eventId, previewImage), cancellationToken);

        return result.ToAspResult(value => Results.Created(string.Empty, value));
    }

    [HttpPut("{eventId:guid}/preview")]
    public async Task<IResult> UpdateImageAsync([FromForm] IFormFile previewImage, Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new UpdateImageCommand(eventId, previewImage), cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpDelete("{eventId:guid}/preview")]
    public async Task<IResult> DeleteImageAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DeleteImageCommand(eventId), cancellationToken);

        return result.ToAspResult(Results.NoContent);
    }
}