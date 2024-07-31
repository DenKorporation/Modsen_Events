using Application.Common.Interfaces.Messaging;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Repositories;
using FluentResults;

namespace Application.UseCases.Events.Queries.GetAllEvents;

public class GetAllEventsHandler(IMapper mapper, IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllEventsQuery, PagedList<EventResponse>>
{
    public async Task<Result<PagedList<EventResponse>>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
    {
        var resultQuery = await unitOfWork.EventRepository.GetAllAsync(cancellationToken);

        var destQuery = resultQuery.ProjectTo<EventResponse>(mapper.ConfigurationProvider);

        return await PagedList<EventResponse>.CreateAsync(destQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}