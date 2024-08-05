using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.Common.Interfaces.Messaging;
using Domain.Errors;
using Domain.Repositories;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Users.Commands.RegisterUserToEvent;

public class RegisterUserToEventHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterUserToEventCommand>
{
    public async Task<Result> Handle(RegisterUserToEventCommand request, CancellationToken cancellationToken)
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

        var placesOccupied = (uint)await unitOfWork.EventUserRepository
            .GetAllUsersFromEvent(resultEvent.Id)
            .CountAsync(cancellationToken);

        if (placesOccupied >= resultEvent.Capacity)
        {
            return new ConflictError("EventCapacity.Conflict", "All seats for the event are occupied");
        }

        await unitOfWork.EventUserRepository.AddUserToEventAsync(request.UserId, request.EventId, cancellationToken);

        var result = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (result.IsFailed)
        {
            return result.Errors.First() switch
            {
                PrimaryKeyError => new ConflictError("UserEvent.Conflict",
                    $"User {request.UserId} has already registered for the event {request.EventId}"),
                _ => new InternalServerError("UserEvent.Create", "Something went wrong when saving the data")
            };
        }

        return Result.Ok();
    }
}