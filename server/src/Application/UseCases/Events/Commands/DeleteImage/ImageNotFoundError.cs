using Application.Common.Errors.Base;

namespace Application.UseCases.Events.Commands.DeleteImage;

public class ImageNotFoundError(string code = "Image.NotFound", string message = "Image not found") : NotFoundError(code, message);