using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.Common.Interfaces.Messaging;
using Application.Services;
using Domain.Repositories;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.Events.Commands.UpdateImage;

public class UpdateImageHandler(
    IUnitOfWork unitOfWork,
    ICloudImageService imageService,
    IConfiguration configuration)
    : ICommandHandler<UpdateImageCommand, UpdateImageResponse>
{
    private readonly string _previewsBucket = configuration.GetRequiredSection("Storage:PreviewsBucket").Value!;

    public async Task<Result<UpdateImageResponse>> Handle(UpdateImageCommand request,
        CancellationToken cancellationToken)
    {
        var dbEvent = await unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (dbEvent is null)
        {
            return new EventNotFoundError(message: $"Event '{request.EventId}' not found");
        }

        var newStoragePath = GetStoragePath(request.EventId, request.Image.FileName);

        var imageResult = await UpdateImageAsync(
            request.Image,
            _previewsBucket,
            dbEvent.ImageStoragePath,
            newStoragePath,
            cancellationToken);

        if (imageResult.IsFailed)
        {
            return imageResult;
        }

        dbEvent.ImageStoragePath = newStoragePath;
        dbEvent.ImageUrl = await imageService.GetImageUrlAsync(_previewsBucket, newStoragePath, cancellationToken);

        await unitOfWork.EventRepository.UpdateAsync(dbEvent, cancellationToken);

        var result = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (result.IsFailed)
        {
            return new InternalServerError("Event.Update", "Something went wrong when saving the data");
        }

        return new UpdateImageResponse { ImageUrl = dbEvent.ImageUrl };
    }

    private async Task<Result> UpdateImageAsync(IFormFile image, string bucketId, string? oldPath,
        string newPath, CancellationToken cancellationToken = default)
    {
        Result result;

        if (oldPath is null)
        {
            result = await imageService.UploadImageAsync(image, bucketId, newPath, cancellationToken);
        }
        else
        {
            await imageService.DeleteImageAsync(bucketId, oldPath, cancellationToken);
            result = await imageService.UpdateImageAsync(image, bucketId, newPath, cancellationToken);
        }

        return result;
    }


    private string GetStoragePath(Guid eventId, string filename)
    {
        var extension = Path.GetExtension(filename);

        return $"{eventId}/preview{extension}";
    }
}