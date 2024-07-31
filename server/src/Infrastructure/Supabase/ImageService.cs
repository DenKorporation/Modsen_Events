using Application.Common.Errors.Base;
using Application.Services;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Supabase.Storage;
using Supabase.Storage.Interfaces;
using Client = Supabase.Client;

namespace Infrastructure.Supabase;

public class ImageService(Client supabaseClient) : ICloudImageService
{
    private readonly IStorageClient<Bucket, FileObject> _storageClient = supabaseClient.Storage;

    public async Task<Result> UploadImageAsync(IFormFile image, string bucketId, string storagePath,
        CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        await image.CopyToAsync(memoryStream, cancellationToken);

        try
        {
            await _storageClient.From(bucketId)
                .Upload(memoryStream.ToArray(), storagePath);

            return Result.Ok();
        }
        catch (Exception)
        {
            return new InternalServerError("Image.Upload", "Error occurred while saving image");
        }
    }

    public async Task<Result> UpdateImageAsync(IFormFile image, string bucketId, string storagePath,
        CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        await image.CopyToAsync(memoryStream, cancellationToken);

        try
        {
            await _storageClient.From(bucketId)
                .Update(memoryStream.ToArray(), storagePath);

            return Result.Ok();
        }
        catch (Exception)
        {
            return new InternalServerError("Image.Update", "Error occurred while saving image");
        }
    }

    public async Task<Result> DeleteImageAsync(string bucketId, string storagePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _storageClient.From(bucketId)
                .Remove(storagePath);

            return Result.Ok();
        }
        catch (Exception)
        {
            return new InternalServerError("Image.Remove", "Error occurred while removing image");
        }
    }

    public Task<string> GetImageUrlAsync(string bucketId, string storagePath,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_storageClient.From(bucketId).GetPublicUrl(storagePath));
    }
}