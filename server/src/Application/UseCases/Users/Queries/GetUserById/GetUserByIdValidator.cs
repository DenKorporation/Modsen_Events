using FluentValidation;

namespace Application.UseCases.Users.Queries.GetUserById;

public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.UserId).NotNull();
    }
}