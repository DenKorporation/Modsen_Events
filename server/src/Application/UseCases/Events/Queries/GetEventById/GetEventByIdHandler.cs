using Application.Common.Errors;
using Application.Common.Interfaces.Messaging;
using AutoMapper;
using Domain.Repositories;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Events.Queries.GetEventById;

public class GetEventByIdHandler(IMapper mapper, IUnitOfWork unitOfWork)
    : IQueryHandler<GetEventByIdQuery, EventResponse>
{
    public async Task<Result<EventResponse>> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var resultEvent = await unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (resultEvent is null)
        {
            return new EventNotFoundError(message: $"Event '{request.EventId}' not found");
        }

        var result = mapper.Map<EventResponse>(resultEvent)!;

        result.PlacesOccupied = (uint)await unitOfWork.EventUserRepository
            .GetAllUsersFromEvent(result.Id)
            .CountAsync(cancellationToken);

        result.IsRegistered = await unitOfWork.EventUserRepository.GetAllUsersFromEvent(result.Id)
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        return result;
    }
}