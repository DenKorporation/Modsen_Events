using Application.Common.Errors;
using Application.Common.Interfaces.Messaging;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Repositories;
using FluentResults;

namespace Application.UseCases.Events.Queries.GetAllUserEvents;

public class GetAllUserEventsHandler(IMapper mapper, IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllUserEventsQuery, PagedList<EventResponse>>
{
    public async Task<Result<PagedList<EventResponse>>> Handle(GetAllUserEventsQuery request,
        CancellationToken cancellationToken)
    {
        var dbUser = await unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (dbUser is null)
        {
            return new UserNotFoundError(message: $"User '{request.UserId}' not found");
        }

        var resultQuery = unitOfWork.EventUserRepository.GetAllEventsFromUser(request.UserId);

        var destQuery =
            resultQuery.ProjectTo<EventResponse>(mapper.ConfigurationProvider, new { userId = request.UserId });

        return await PagedList<EventResponse>.CreateAsync(destQuery, request.PageNumber, request.PageSize,
            cancellationToken);
    }
}