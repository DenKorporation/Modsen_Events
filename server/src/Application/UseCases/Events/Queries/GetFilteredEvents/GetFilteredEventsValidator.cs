using FluentValidation;

namespace Application.UseCases.Events.Queries.GetFilteredEvents;

public class GetFilteredEventsValidator : AbstractValidator<GetFilteredEventsQuery>
{
    public GetFilteredEventsValidator()
    {
        RuleFor(x => x.PageNumber).NotNull().GreaterThan(0);

        RuleFor(x => x.PageSize).NotNull().GreaterThan(0);

        RuleFor(x => x.Name).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Address).MaximumLength(150).When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.Category).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Category));
    }
}