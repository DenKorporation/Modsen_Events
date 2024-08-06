using Application.Common.Errors;
using Application.Common.Interfaces.Messaging;
using AutoMapper;
using Domain.Repositories;
using FluentResults;

namespace Application.UseCases.Users.Queries.GetUserById;

public class GetUserByIdHandler(IMapper mapper, IUnitOfWork unitOfWork)
    : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var resultUser = await unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (resultUser is null)
        {
            return new UserNotFoundError(message: $"User '{request.UserId}' not found");
        }

        return mapper.Map<UserResponse>(resultUser)!;
    }
}