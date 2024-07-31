using Application.Common.Interfaces.Messaging;

namespace Application.UseCases.Events.Queries.GetEventById;

public record GetEventByIdQuery(Guid EventId) : IQuery<EventResponse>;