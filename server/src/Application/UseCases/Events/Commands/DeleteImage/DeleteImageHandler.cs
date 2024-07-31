using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.Common.Interfaces.Messaging;
using Application.Services;
using Domain.Repositories;
using FluentResults;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.Events.Commands.DeleteImage;

public class DeleteImageHandler(
    IUnitOfWork unitOfWork,
    ICloudImageService imageService,
    IConfiguration configuration) : ICommandHandler<DeleteImageCommand>
{
    private readonly string _previewsBucket = configuration.GetRequiredSection("Storage:PreviewsBucket").Value!;

    public async Task<Result> Handle(DeleteImageCommand request,
        CancellationToken cancellationToken)
    {
        var dbEvent = await unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (dbEvent is null)
        {
            return new EventNotFoundError(message: $"Event '{request.EventId}' not found");
        }

        if (dbEvent.ImageStoragePath is null)
        {
            return new ImageNotFoundError(message: $"Image for event {dbEvent.Id} not found");
        }

        var deleteResult = await imageService.DeleteImageAsync(
            _previewsBucket,
            dbEvent.ImageStoragePath!,
            cancellationToken);

        if (deleteResult.IsFailed)
        {
            return deleteResult;
        }

        dbEvent.ImageStoragePath = null;
        dbEvent.ImageUrl = null;

        await unitOfWork.EventRepository.UpdateAsync(dbEvent, cancellationToken);

        var result = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (result.IsFailed)
        {
            return new InternalServerError("Event.Update", "Something went wrong when saving the data");
        }

        return Result.Ok();
    }
}