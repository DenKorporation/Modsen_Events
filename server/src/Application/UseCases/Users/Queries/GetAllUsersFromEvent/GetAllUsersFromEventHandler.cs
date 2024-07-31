using Application.Common.Errors;
using Application.Common.Interfaces.Messaging;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Repositories;
using FluentResults;

namespace Application.UseCases.Users.Queries.GetAllUsersFromEvent;

public class GetAllUsersFromEventHandler(IMapper mapper, IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllUsersFromEventQuery, PagedList<UserResponse>>
{
    public async Task<Result<PagedList<UserResponse>>> Handle(GetAllUsersFromEventQuery request,
        CancellationToken cancellationToken)
    {
        var dbEvent = await unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (dbEvent is null)
        {
            return new EventNotFoundError(message: $"Event '{request.EventId}' not found");
        }

        var resultQuery = unitOfWork.EventUserRepository.GetAllUsersFromEvent(request.EventId);

        var destQuery =
            resultQuery.ProjectTo<UserResponse>(mapper.ConfigurationProvider, new { eventId = request.EventId });

        return await PagedList<UserResponse>.CreateAsync(destQuery, request.PageNumber, request.PageSize,
            cancellationToken);
    }
}