using Application.Common.Errors.Base;

namespace Application.Common.Errors;

public class EventUserNotFoundError(string code = "EventUser.NotFound", string message = "EventUser not found")
    : NotFoundError(code, message);