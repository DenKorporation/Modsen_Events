using Domain.Constants;
using FluentValidation;

namespace Application.UseCases.Users.Commands.AssignRoleToUser;

public class AssignRoleToUserValidator : AbstractValidator<AssignRoleToUserCommand>
{
    private readonly string[] _roles = [Roles.Registered, Roles.Administrator];

    public AssignRoleToUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotNull();

        RuleFor(x => x.Role)
            .NotEmpty()
            .MaximumLength(256)
            .Must(r => _roles.Any(role => string.Equals(role, r, StringComparison.InvariantCultureIgnoreCase)))
            .WithMessage("There is no such role");
    }
}