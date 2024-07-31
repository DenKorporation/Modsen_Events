using Application.Common.Interfaces.Messaging;
using Application.Common.Models;

namespace Application.UseCases.Users.Queries.GetAllUsersFromEvent;

public record GetAllUsersFromEventQuery(Guid EventId, int PageNumber, int PageSize) : IQuery<PagedList<UserResponse>>;