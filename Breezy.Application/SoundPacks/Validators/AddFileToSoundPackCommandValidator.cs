using Breezy.Application.SoundPacks.Commands;
using FluentValidation;

namespace Breezy.Application.SoundPacks.Validators;

public class AddFileToSoundPackCommandValidator : AbstractValidator<AddFileToSoundPackCommand>
{
    public AddFileToSoundPackCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty");
        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("FileName cannot be empty")
            .MaximumLength(255)
            .WithMessage("FileName cannot exceed 255 characters");
        RuleFor(x => x.SoundPackId)
            .NotEmpty()
            .WithMessage("SoundPackId cannot be empty");
        RuleFor(x => x.BPM)
            .GreaterThanOrEqualTo(40)
            .WithMessage("BPM needs to be greater or equal to 40")
            .LessThanOrEqualTo(300)
            .WithMessage("BPM needs to be less or equal to 300");
        RuleFor(x => x.SoundCategory)
            .IsInEnum()
            .WithMessage("Invalid sound category");
        RuleFor(x => x.Key)
            .MaximumLength(10)
            .WithMessage("Key cannot exceed 10 characters");
        RuleFor(x => x.FileType)
            .MaximumLength(255)
            .WithMessage("FileType cannot exceed 255 characters");
        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .WithMessage("FileSize needs to be greater than 0");
        RuleFor(x => x.DurationSeconds)
            .GreaterThan(0)
            .WithMessage("DurationSeconds needs to be greater than 0");
        RuleFor(x => x.FileContent)
            .NotEmpty()
            .WithMessage("FileContent cannot be empty");
    }
}