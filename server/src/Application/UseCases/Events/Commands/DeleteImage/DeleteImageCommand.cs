using Application.Common.Interfaces.Messaging;

namespace Application.UseCases.Events.Commands.DeleteImage;

public record DeleteImageCommand(Guid EventId) : ICommand;