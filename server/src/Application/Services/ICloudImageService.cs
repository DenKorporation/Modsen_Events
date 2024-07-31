using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public interface ICloudImageService
{
    public Task<Result> UploadImageAsync(IFormFile image, string bucketId, string storagePath,
        CancellationToken cancellationToken = default);

    public Task<Result> UpdateImageAsync(IFormFile image, string bucketId, string storagePath,
        CancellationToken cancellationToken = default);

    public Task<Result> DeleteImageAsync(string bucketId, string storagePath,
        CancellationToken cancellationToken = default);

    public Task<string> GetImageUrlAsync(string bucketId, string storagePath,
        CancellationToken cancellationToken = default);
}