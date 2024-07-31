using FluentValidation;

namespace Application.UseCases.Users.Queries.GetAllUsersFromEvent;

public class GetAllUsersFromEventValidator : AbstractValidator<GetAllUsersFromEventQuery>
{
    public GetAllUsersFromEventValidator()
    {
        RuleFor(x => x.EventId).NotNull();

        RuleFor(x => x.PageNumber).NotNull().GreaterThan(0);

        RuleFor(x => x.PageSize).NotNull().GreaterThan(0);
    }
}