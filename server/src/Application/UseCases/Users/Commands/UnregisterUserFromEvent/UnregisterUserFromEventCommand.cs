using Application.Common.Interfaces.Messaging;

namespace Application.UseCases.Users.Commands.UnregisterUserFromEvent;

public record UnregisterUserFromEventCommand(Guid UserId, Guid EventId) : ICommand;