using FluentValidation;

namespace Application.UseCases.Users.Queries.GetAllUsers;

public class GetAllUsersValidator : AbstractValidator<GetAllUsersQuery>
{
    public GetAllUsersValidator()
    {
        RuleFor(x => x.PageNumber).NotNull().GreaterThan(0);

        RuleFor(x => x.PageSize).NotNull().GreaterThan(0);
    }
}