using Application.Common.Errors.Base;

namespace Application.UseCases.Events.Commands.UpdateEvent;

public class EventCapacityConflictError(
    string code = "EventCapacity.Conflict",
    string message = "The new capacity value conflicts with the state of the database")
    : ConflictError(code, message);