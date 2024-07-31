using Application.Common.Interfaces.Messaging;

namespace Application.UseCases.Events.Commands.UpdateEvent;

public record UpdateEventCommand(
    Guid Id,
    string Name,
    string Description,
    DateTime Date,
    string Address,
    string Category,
    uint Capacity) : ICommand<EventResponse>;