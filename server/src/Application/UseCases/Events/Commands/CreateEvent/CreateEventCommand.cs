using Application.Common.Interfaces.Messaging;

namespace Application.UseCases.Events.Commands.CreateEvent;

public record CreateEventCommand(
    string Name,
    string Description,
    DateTime Date,
    string Address,
    string Category,
    uint Capacity) : ICommand<EventResponse>;