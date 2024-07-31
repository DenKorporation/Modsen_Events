using Application.Common.Interfaces.Messaging;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Events.Commands.UploadImage;

public record UploadImageCommand(Guid EventId, IFormFile Image) : ICommand<UploadImageResponse>;