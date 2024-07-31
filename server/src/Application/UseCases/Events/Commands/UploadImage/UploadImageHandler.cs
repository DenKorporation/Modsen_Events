using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.Common.Interfaces.Messaging;
using Application.Services;
using Domain.Repositories;
using FluentResults;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.Events.Commands.UploadImage;

public class UploadImageHandler(
    IUnitOfWork unitOfWork,
    ICloudImageService imageService,
    IConfiguration configuration)
    : ICommandHandler<UploadImageCommand, UploadImageResponse>
{
    private readonly string _previewsBucket = configuration.GetRequiredSection("Storage:PreviewsBucket").Value!;

    public async Task<Result<UploadImageResponse>> Handle(UploadImageCommand request,
        CancellationToken cancellationToken)
    {
        var dbEvent = await unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (dbEvent is null)
        {
            return new EventNotFoundError(message: $"Event '{request.EventId}' not found");
        }

        if (dbEvent.ImageStoragePath is not null)
        {
            return new UploadImageConflictError(message: $"Image for event {dbEvent.Id} already exists");
        }

        var storagePath = GetStoragePath(request.EventId, request.Image.FileName);

        var imageResult = await imageService.UploadImageAsync(
            request.Image,
            _previewsBucket,
            storagePath,
            cancellationToken);

        if (imageResult.IsFailed)
        {
            return imageResult;
        }

        dbEvent.ImageStoragePath = storagePath;
        dbEvent.ImageUrl = await imageService.GetImageUrlAsync(_previewsBucket, storagePath, cancellationToken);

        await unitOfWork.EventRepository.UpdateAsync(dbEvent, cancellationToken);

        var result = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (result.IsFailed)
        {
            return new InternalServerError("Event.Update", "Something went wrong when saving the data");
        }

        return new UploadImageResponse { ImageUrl = dbEvent.ImageUrl };
    }

    private static string GetStoragePath(Guid eventId, string filename)
    {
        var extension = Path.GetExtension(filename);

        return $"{eventId}/preview{extension}";
    }
}