using Application.Common.Errors.Base;

namespace Application.Common.Errors;

public class ValidationError(
    Dictionary<string, string[]> errors,
    string code = "Validation",
    string message = "Validation errors occurred")
    : BadRequestError(code, message)
{
    public Dictionary<string, string[]> ValidationErrors { get; } = errors;
}