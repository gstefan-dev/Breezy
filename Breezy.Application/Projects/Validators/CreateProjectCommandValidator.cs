using Breezy.Application.Projects.Commands;
using FluentValidation;

namespace Breezy.Application.Projects.Validators;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("OwnerId cannot be empty");
        RuleFor(x => x.ProjectName)
            .NotEmpty()
            .WithMessage("ProjectName cannot be empty")
            .MaximumLength(100)
            .WithMessage("ProjectName can have maximum 100 characters");
        RuleFor(x => x.LocalPath)
            .NotEmpty()
            .WithMessage("LocalPath cannot be empty");
        RuleFor(x=> x.Hash)
            .NotEmpty()
            .WithMessage("Hash cannot be empty")
            .Matches(@"^[A-F0-9]{64}$")
            .WithMessage("Hash must contain only hexadecimal characters (A-F, 0-9)");
        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .WithMessage("FileSize needs to be greater than 0");
        RuleFor(x => x.FileContent)
            .NotEmpty()
            .WithMessage("FileContent cannot be empty");
        RuleFor(x => x.FileType)
            .NotEmpty()
            .WithMessage("FileType cannot be empty");
    }
}