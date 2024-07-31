using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.Common.Interfaces.Messaging;
using AutoMapper;
using Domain.Repositories;
using FluentResults;

namespace Application.UseCases.Events.Commands.DeleteEvent;

public class DeleteEventHandler(IMapper mapper, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteEventCommand>
{
    public async Task<Result> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var dbEvent = await unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (dbEvent is null)
        {
            return new EventNotFoundError(message: $"Event '{request.EventId}' not found");
        }

        await unitOfWork.EventRepository.DeleteAsync(dbEvent, cancellationToken);

        var result = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (result.IsFailed)
        {
            return new InternalServerError("Event.Delete", "Something went wrong when removing the data");
        }

        return Result.Ok();
    }
}