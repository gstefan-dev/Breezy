using Breezy.Application.Sessions.Commands;
using FluentValidation;

namespace Breezy.Application.Sessions.Validators;

public class SubmitSampleCommandValidator : AbstractValidator<SubmitSampleCommand>
{
    public SubmitSampleCommandValidator()
    {
        RuleFor(x => x.SessionCode)
            .NotEmpty()
            .WithMessage("SessionCode cannot be empty")
            .Length(4, 10)
            .WithMessage("SessionCode must have between 4 and 10 characters")
            .Matches(@"^[A-Z0-9]+$")
            .WithMessage("SessionCode must contain only uppercase letters and numbers");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("FileName cannot be empty")
            .MaximumLength(255)
            .WithMessage("FileName cannot exceed 255 characters");
        
        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .WithMessage("FileSize must be greater than 0");
        
        RuleFor(x => x.DurationSeconds)
            .GreaterThan(0)
            .WithMessage("DurationSeconds must be greater than 0");
        
        RuleFor(x => x.BPM)
            .GreaterThanOrEqualTo(40)
            .WithMessage("BPM must be between 40 and 300")
            .LessThanOrEqualTo(300)
            .WithMessage("BPM must be between 40 and 300");
        
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("Key cannot be empty");
        
        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("ContentType cannot be empty");
    }
}