using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.Common.Interfaces.Messaging;
using Domain.Repositories;
using FluentResults;

namespace Application.UseCases.Users.Commands.DeleteUser;

public class DeleteUserHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteUserCommand>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var dbUser = await unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (dbUser is null)
        {
            return new UserNotFoundError(message: $"User '{request.UserId}' not found");
        }

        await unitOfWork.UserRepository.DeleteAsync(dbUser, cancellationToken);

        var result = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (result.IsFailed)
        {
            return new InternalServerError("User.Delete", "Something went wrong when removing the data");
        }

        return Result.Ok();
    }
}