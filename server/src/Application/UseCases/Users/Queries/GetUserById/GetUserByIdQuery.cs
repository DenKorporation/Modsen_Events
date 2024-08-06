using Application.Common.Interfaces.Messaging;

namespace Application.UseCases.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;