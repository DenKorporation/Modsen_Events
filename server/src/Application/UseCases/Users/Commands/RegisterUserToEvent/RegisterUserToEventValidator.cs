using FluentValidation;

namespace Application.UseCases.Users.Commands.RegisterUserToEvent;

public class RegisterUserToEventValidator : AbstractValidator<RegisterUserToEventCommand>
{
    public RegisterUserToEventValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.EventId).NotEmpty();
    }
}