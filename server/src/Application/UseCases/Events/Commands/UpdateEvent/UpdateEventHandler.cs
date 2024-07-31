using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.Common.Interfaces.Messaging;
using AutoMapper;
using Domain.Repositories;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Events.Commands.UpdateEvent;

public class UpdateEventHandler(IMapper mapper, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateEventCommand, EventResponse>
{
    public async Task<Result<EventResponse>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var dbEvent = await unitOfWork.EventRepository.GetByIdAsync(request.Id, cancellationToken);

        if (dbEvent is null)
        {
            return new EventNotFoundError(message: $"Event '{request.Id}' not found");
        }

        var placesOccupied = (uint)await unitOfWork.EventUserRepository
            .GetAllUsersFromEvent(dbEvent.Id)
            .CountAsync(cancellationToken);

        if (request.Capacity < placesOccupied)
        {
            return new EventCapacityConflictError(
                message: $"More than {request.Capacity} users have already registered for the event");
        }

        mapper.Map(request, dbEvent);

        await unitOfWork.EventRepository.UpdateAsync(dbEvent, cancellationToken);

        var result = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (result.IsFailed)
        {
            return new InternalServerError("Event.Update", "Something went wrong when saving the data");
        }

        var response = mapper.Map<EventResponse>(dbEvent)!;
        response.PlacesOccupied = placesOccupied;
        return response;
    }
}