using FluentResults;
using MediatR;

namespace Application.Common.Interfaces.Messaging;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;

public interface ICommand : IRequest<Result>;