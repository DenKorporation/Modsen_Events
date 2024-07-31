using Application.Common.Interfaces.Messaging;
using Application.Common.Models;

namespace Application.UseCases.Events.Queries.GetFilteredEvents;

public record GetFilteredEventsQuery(
    int PageNumber,
    int PageSize,
    string? Name,
    string? Address,
    string? Category,
    DateTime? StartDate,
    DateTime? EndDate) : IQuery<PagedList<EventResponse>>;