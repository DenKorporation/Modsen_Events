using Application.Common.Errors.Base;

namespace Application.Common.Errors;

public class EventNotFoundError(string code = "Event.NotFound", string message = "Event not found") : NotFoundError(code, message);