using Application.Common.Interfaces.Messaging;
using Application.Common.Models;

namespace Application.UseCases.Events.Queries.GetAllEvents;

public record GetAllEventsQuery(int PageNumber, int PageSize) : IQuery<PagedList<EventResponse>>;