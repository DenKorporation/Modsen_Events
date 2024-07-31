namespace Application.Common.Errors.Base;

public class BadRequestError(string code, string message) : BaseError(code, message);