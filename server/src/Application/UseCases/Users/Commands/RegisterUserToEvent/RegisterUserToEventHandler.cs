using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.Common.Interfaces.Messaging;
using Domain.Errors;
using Domain.Repositories;
using FluentResults;

namespace Application.UseCases.Users.Commands.RegisterUserToEvent;

public class RegisterUserToEventHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterUserToEventCommand>
{
    public async Task<Result> Handle(RegisterUserToEventCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.EventUserRepository.AddUserToEventAsync(request.UserId, request.EventId, cancellationToken);

        var result = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (result.IsFailed)
        {
            return result.Errors.First() switch
            {
                ForeignKeyError { ForeignTable: "Events" } => new EventNotFoundError(
                    message: $"Event {request.EventId} not found"),
                ForeignKeyError { ForeignTable: "Users" } => new UserNotFoundError(
                    message: $"User {request.UserId} not found"),
                PrimaryKeyError => new ConflictError("UserEvent.Conflict",
                    $"User {request.UserId} has already registered for the event {request.EventId}"),
                _ => new InternalServerError("UserEvent.Create", "Something went wrong when saving the data")
            };
        }

        return Result.Ok();
    }
}