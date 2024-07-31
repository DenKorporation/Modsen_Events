using Application.Common.Errors.Base;

namespace Application.UseCases.Events.Commands.UploadImage;

public class UploadImageConflictError(
    string code = "ImageUpload.Conflict",
    string message = "Image for this event already exists") : ConflictError(code, message);