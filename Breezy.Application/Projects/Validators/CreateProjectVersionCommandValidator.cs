using Breezy.Application.Projects.Commands;
using FluentValidation;

namespace Breezy.Application.Projects.Validators;

public class CreateProjectVersionCommandValidator : AbstractValidator<CreateProjectVersionCommand>
{
    public CreateProjectVersionCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId cant be empty");
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId cant be empty");
        RuleFor(x => x.FileContent)
            .NotEmpty()
            .WithMessage("FileContent cant be empty");
        RuleFor(x => x.NewHash)
            .NotEmpty()
            .Matches(@"^[A-F0-9]{64}$")
            .WithMessage("Hash must contain only hexadecimal characters (A-F, 0-9)");
        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .WithMessage("FileSize needs to be greater than 0");
        RuleFor(x => x.FileType)
            .NotEmpty()
            .WithMessage("FileType cant be empty");
    }
}