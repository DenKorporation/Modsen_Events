using Application.Common.Interfaces.Messaging;

namespace Application.UseCases.Events.Queries.GetEventByName;

public record GetEventByNameQuery(string Name) : IQuery<EventResponse>;