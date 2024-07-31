using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.Common.Interfaces.Messaging;
using Domain.Repositories;
using FluentResults;

namespace Application.UseCases.Users.Commands.UnregisterUserFromEvent;

public class UnregisterUserFromEventHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UnregisterUserFromEventCommand>
{
    public async Task<Result> Handle(UnregisterUserFromEventCommand request, CancellationToken cancellationToken)
    {
        var removeResult = await unitOfWork.EventUserRepository.RemoveUserFromEventAsync(
            request.UserId,
            request.EventId,
            cancellationToken);

        if (!removeResult)
        {
            return new EventUserNotFoundError(
                message: $"User {request.UserId} is not registered for the event {request.EventId}");
        }

        var result = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (result.IsFailed)
        {
            return new InternalServerError("EventUser.Delete", "Something went wrong when removing the data");
        }

        return Result.Ok();
    }
}