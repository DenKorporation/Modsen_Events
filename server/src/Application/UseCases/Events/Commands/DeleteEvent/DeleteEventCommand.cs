using Application.Common.Interfaces.Messaging;

namespace Application.UseCases.Events.Commands.DeleteEvent;

public record DeleteEventCommand(Guid EventId) : ICommand;