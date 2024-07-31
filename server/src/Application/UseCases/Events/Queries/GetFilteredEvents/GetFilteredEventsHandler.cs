using Application.Common.Extensions;
using Application.Common.Interfaces.Messaging;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Repositories;
using FluentResults;

namespace Application.UseCases.Events.Queries.GetFilteredEvents;

public class GetFilteredEventsHandler(IMapper mapper, IUnitOfWork unitOfWork)
    : IQueryHandler<GetFilteredEventsQuery, PagedList<EventResponse>>
{
    public async Task<Result<PagedList<EventResponse>>> Handle(GetFilteredEventsQuery request,
        CancellationToken cancellationToken)
    {
        var resultQuery = await unitOfWork.EventRepository.GetAllAsync(cancellationToken);

        resultQuery = resultQuery
            .FilterByNameIfSet(request.Name)
            .FilterByAddressIfSet(request.Address)
            .FilterByCategoryIfSet(request.Category)
            .FilterByDateIfSet(request.StartDate, request.EndDate);

        var destQuery = resultQuery.ProjectTo<EventResponse>(mapper.ConfigurationProvider);

        return await PagedList<EventResponse>.CreateAsync(destQuery, request.PageNumber, request.PageSize,
            cancellationToken);
    }
}