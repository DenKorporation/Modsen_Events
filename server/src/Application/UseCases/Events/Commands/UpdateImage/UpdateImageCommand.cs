using Application.Common.Interfaces.Messaging;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Events.Commands.UpdateImage;

public record UpdateImageCommand(Guid EventId, IFormFile Image) : ICommand<UpdateImageResponse>;