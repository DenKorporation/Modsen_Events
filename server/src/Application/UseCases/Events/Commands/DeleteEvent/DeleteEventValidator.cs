using FluentValidation;

namespace Application.UseCases.Events.Commands.DeleteEvent;

public class DeleteEventValidator : AbstractValidator<DeleteEventCommand>
{
    public DeleteEventValidator()
    {
        RuleFor(x => x.EventId).NotNull();
    }
}