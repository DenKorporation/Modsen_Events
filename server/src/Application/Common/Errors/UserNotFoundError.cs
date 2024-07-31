using Application.Common.Errors.Base;

namespace Application.Common.Errors;

public class UserNotFoundError(string code = "User.NotFound", string message = "User not found")
    : NotFoundError(code, message);