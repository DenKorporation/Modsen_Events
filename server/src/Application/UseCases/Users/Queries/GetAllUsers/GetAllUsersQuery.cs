using Application.Common.Interfaces.Messaging;
using Application.Common.Models;

namespace Application.UseCases.Users.Queries.GetAllUsers;

public record GetAllUsersQuery(int PageNumber, int PageSize) : IQuery<PagedList<UserResponse>>;