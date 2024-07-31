using FluentValidation;

namespace Application.UseCases.Events.Queries.GetEventById;

public class GetEventByIdValidator : AbstractValidator<GetEventByIdQuery>
{
    public GetEventByIdValidator()
    {
        RuleFor(x => x.EventId).NotNull();
    }
}