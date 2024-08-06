using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.Common.Interfaces.Messaging;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Repositories;
using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Users.Commands.CreateUser;

public class CreateUserHandler(IMapper mapper, IUnitOfWork unitOfWork, UserManager<User> userManager)
    : ICommandHandler<CreateUserCommand, UserResponse>
{
    private readonly string[] _passwordValidationErrorCodes =
    [
        "PasswordTooShort", "PasswordRequiresUpper", "PasswordRequiresLower", "PasswordRequiresNonAlphanumeric",
        "PasswordRequiresDigit", "PasswordRequiresUniqueChars"
    ];

    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var createUser = mapper.Map<User>(request);

        var result = await unitOfWork.UserRepository.CreateAsync(createUser, request.Password, cancellationToken);

        if (!result.Succeeded)
        {
            // Email is equivalent to UserName
            var duplicateEmailError =
                result.Errors.FirstOrDefault(e => e.Code is "DuplicateEmail" or "DuplicateUserName");
            if (duplicateEmailError is not null)
            {
                return new ConflictError("UserEmail.Conflict", duplicateEmailError.Description);
            }

            var passwordValidationErrors =
                result.Errors.Where(e => _passwordValidationErrorCodes.Contains(e.Code)).ToList();

            if (passwordValidationErrors.Any())
            {
                Dictionary<string, string[]> errors = new Dictionary<string, string[]>
                    { { "Password", passwordValidationErrors.Select(e => e.Description).ToArray() } };
                return new ValidationError(code: "CreateUser.Validation", errors: errors);
            }

            return new InternalServerError("User.Create", "Something went wrong when saving the data");
        }

        result = await AssignRegisteredRoleAsync(createUser);
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
        
        return mapper.Map<UserResponse>(createUser);
    }

    private async Task<IdentityResult> AssignRegisteredRoleAsync(User user)
    {
        return await userManager.AddToRoleAsync(user, Roles.Registered);
    }
}