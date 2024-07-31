using FluentValidation;

namespace Application.UseCases.Events.Commands.CreateEvent;

public class CreateEventValidator: AbstractValidator<CreateEventCommand>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Description).NotEmpty().MaximumLength(250);

        RuleFor(x => x.Date).NotNull();

        RuleFor(x => x.Address).NotEmpty().MaximumLength(150);

        RuleFor(x => x.Category).NotEmpty().MaximumLength(50);

        RuleFor(x => x.Capacity).NotNull();
    }    
}