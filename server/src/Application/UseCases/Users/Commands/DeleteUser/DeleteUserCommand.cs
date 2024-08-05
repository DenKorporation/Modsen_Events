using Application.Common.Interfaces.Messaging;

namespace Application.UseCases.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid UserId) : ICommand;