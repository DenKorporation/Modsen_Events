using Application.Common.Interfaces.Messaging;
using Application.Common.Models;

namespace Application.UseCases.Events.Queries.GetAllUserEvents;

public record GetAllUserEventsQuery(Guid UserId, int PageNumber, int PageSize) : IQuery<PagedList<EventResponse>>;