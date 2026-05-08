using Breezy.Application.SoundPacks.Commands;
using FluentValidation;

namespace Breezy.Application.SoundPacks.Validators;

public class ExportSoundPackCommandValidator : AbstractValidator<ExportSoundPackCommand>
{
    public ExportSoundPackCommandValidator()
    {
        RuleFor(x => x.SoundPackId)
            .NotEmpty()
            .WithMessage("SoundPackId cannot be empty");
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty");
        RuleFor(x => x.ExportFormat)
            .IsInEnum()
            .WithMessage("Invalid Export format");
    }
}