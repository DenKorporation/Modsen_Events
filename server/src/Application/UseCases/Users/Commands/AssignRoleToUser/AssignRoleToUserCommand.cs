using Application.Common.Interfaces.Messaging;

namespace Application.UseCases.Users.Commands.AssignRoleToUser;

public record AssignRoleToUserCommand(
    Guid UserId,
    string Role) : ICommand;