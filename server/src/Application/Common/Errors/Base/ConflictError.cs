namespace Application.Common.Errors.Base;

public class ConflictError(string code, string message) : BaseError(code, message);