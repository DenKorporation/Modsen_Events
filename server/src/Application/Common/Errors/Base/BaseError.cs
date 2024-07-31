using FluentResults;

namespace Application.Common.Errors.Base;

public abstract class BaseError(string code, string message): Error(message)
{
    public string Code { get; set; } = code;
}