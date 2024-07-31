using FluentValidation;

namespace Application.UseCases.Users.Commands.UnregisterUserFromEvent;

public class UnregisterUserFromEventValidator : AbstractValidator<UnregisterUserFromEventCommand>
{
    public UnregisterUserFromEventValidator()
    {
        RuleFor(x => x.UserId).NotNull();
        
        RuleFor(x => x.EventId).NotNull();
    }
}