using FluentValidation;

namespace Application.UseCases.Events.Queries.GetAllEvents;

public class GetAllEventsValidator : AbstractValidator<GetAllEventsQuery>
{
    public GetAllEventsValidator()
    {
        RuleFor(x => x.PageNumber).NotNull().GreaterThan(0);

        RuleFor(x => x.PageSize).NotNull().GreaterThan(0);
    }
}