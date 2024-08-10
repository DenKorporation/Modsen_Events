using FluentValidation;

namespace Application.UseCases.Events.Queries.GetEventByName;

public class GetEventByNameValidator : AbstractValidator<GetEventByNameQuery>
{
    public GetEventByNameValidator()
    {
        // from db configuration constraint
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.UserId).NotNull();
    }
}