using FluentValidation;

namespace Application.UseCases.Events.Queries.GetAllUserEvents;

public class GetAllUserEventsValidator : AbstractValidator<GetAllUserEventsQuery>
{
    public GetAllUserEventsValidator()
    {
        RuleFor(x => x.UserId).NotNull();

        RuleFor(x => x.PageNumber).NotNull().GreaterThan(0);

        RuleFor(x => x.PageSize).NotNull().GreaterThan(0);
    }
}