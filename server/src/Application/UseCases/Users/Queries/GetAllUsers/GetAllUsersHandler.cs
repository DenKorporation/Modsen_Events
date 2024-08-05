using Application.Common.Interfaces.Messaging;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Repositories;
using FluentResults;

namespace Application.UseCases.Users.Queries.GetAllUsers;

public class GetAllUsersHandler(IMapper mapper, IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllUsersQuery, PagedList<UserResponse>>
{
    public async Task<Result<PagedList<UserResponse>>> Handle(GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var resultQuery = await unitOfWork.UserRepository.GetAllAsync(cancellationToken);

        var destQuery = resultQuery.ProjectTo<UserResponse>(mapper.ConfigurationProvider);

        return await PagedList<UserResponse>.CreateAsync(destQuery, request.PageNumber, request.PageSize,
            cancellationToken);
    }
}