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
        var resultUser = await unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (resultUser is null)
        {
            return new UserNotFoundError(message: $"User '{request.UserId}' not found");
        }
        
        var resultEvent = await unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (resultEvent is null)
        {
            return new EventNotFoundError(message: $"Event '{request.EventId}' not found");
        }
        
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