using FluentValidation;

namespace Application.UseCases.Events.Commands.DeleteImage;

public class DeleteImageValidator : AbstractValidator<DeleteImageCommand>
{
    public DeleteImageValidator()
    {
        RuleFor(x => x.EventId).NotNull();
    }
}