using Breezy.Application.Sessions.Commands;
using FluentValidation;

namespace Breezy.Application.Sessions.Validators;

public class AddParticipantCommandValidator : AbstractValidator<AddParticipantCommand>
{
    public AddParticipantCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty");
        RuleFor(x => x.SessionCode)
            .NotEmpty()
            .WithMessage("SessionCode cannot be empty")
            .Length(4, 10)
            .WithMessage("SessionCode must have between 4 and 10 characters")
            .Matches(@"^[A-Z0-9]+$")
            .WithMessage("SessionCode must contain only uppercase letters and numbers");


    }
}