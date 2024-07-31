using Application.Common.Errors;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviours;

public sealed class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : ResultBase<TResponse>, new()
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var errorsDictionary = validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = errorMessages.Distinct().ToArray()
                })
            .ToDictionary(x => x.Key, x => x.Values);

        if (errorsDictionary.Any())
        {
            var result = new TResponse();
            result.WithError(new ValidationError(errorsDictionary, GetValidationErrorCode(typeof(TRequest))));
            return result;
        }

        return await next();
    }

    private string GetValidationErrorCode(Type type)
    {
        string typeName = type.Name;

        string validationType = typeName
            .Replace("Command", "")
            .Replace("Query", "");

        return $"{validationType}.Validation";
    }
}