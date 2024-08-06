using Application.Common.Interfaces.Messaging;

namespace Application.UseCases.Users.Commands.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Birthday) : ICommand<UserResponse>;