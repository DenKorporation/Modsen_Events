using FluentValidation;

namespace Application.UseCases.Events.Commands.UploadImage;

public class UploadImageValidator : AbstractValidator<UploadImageCommand>
{
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png"];
    private readonly int _fiftyMegabytes = 50 * (int)Math.Pow(2, 20);

    public UploadImageValidator()
    {
        RuleFor(x => x.EventId).NotNull();

        RuleFor(x => x.Image)
            .NotNull()
            .Must(image => image.Length <= _fiftyMegabytes)
            .WithMessage("The image size should not be larger than 50 Mb")
            .Must(image => _allowedExtensions.Contains(Path.GetExtension(image.FileName)))
            .WithMessage("Invalid image format. Try .jpg, .jpeg or .png");
    }
}