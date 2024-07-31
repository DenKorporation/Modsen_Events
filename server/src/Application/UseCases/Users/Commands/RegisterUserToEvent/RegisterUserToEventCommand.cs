using Application.Common.Interfaces.Messaging;

namespace Application.UseCases.Users.Commands.RegisterUserToEvent;

public record RegisterUserToEventCommand(Guid UserId, Guid EventId) : ICommand;