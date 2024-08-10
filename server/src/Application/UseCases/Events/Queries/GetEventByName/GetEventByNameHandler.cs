using Application.Common.Errors;
using Application.Common.Interfaces.Messaging;
using AutoMapper;
using Domain.Repositories;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Events.Queries.GetEventByName;

public class GetEventByNameHandler(IMapper mapper, IUnitOfWork unitOfWork)
    : IQueryHandler<GetEventByNameQuery, EventResponse>
{
    public async Task<Result<EventResponse>> Handle(GetEventByNameQuery request, CancellationToken cancellationToken)
    {
        var resultEvent = await unitOfWork.EventRepository.GetByNameAsync(request.Name, cancellationToken);
        
        if (resultEvent is null)
        {
            return new EventNotFoundError(message: $"Event '{request.Name}' not found");
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