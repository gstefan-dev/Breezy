using Breezy.Application.SoundPacks.Commands;
using FluentValidation;

namespace Breezy.Application.SoundPacks.Validators;

public class CreateSoundPackCommandValidator : AbstractValidator<CreateSoundPackCommand>
{
    public CreateSoundPackCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty");
        RuleFor(x => x.PackName)
            .NotEmpty()
            .WithMessage("PackName cannot be empty")
            .MaximumLength(100)
            .WithMessage("PackName cannot exceed 100 characters");
        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");
        RuleFor(x => x.Genre)
            .MaximumLength(50)
            .WithMessage("Genre cannot exceed 50 characters");
        RuleFor(x => x.Mood)
            .MaximumLength(50)
            .WithMessage("Mood cannot exceed 50 characters");
        RuleFor(x => x.PackCategory)
            .MaximumLength(50)
            .WithMessage("PackCategory cannot exceed 50 characters");
        
    }
}