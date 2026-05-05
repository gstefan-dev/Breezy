using Breezy.Application.Sessions.Commands;
using FluentValidation;

namespace Breezy.Application.Sessions.Validators;

public class CreateCollaborationSessionCommandValidator : AbstractValidator<CreateCollaborationSessionCommand>
{
    public CreateCollaborationSessionCommandValidator()
    {
        RuleFor(x => x.HostId)
            .NotEmpty()
                .WithMessage("HostId cannot be empty");
        RuleFor(x => x.SessionName)
            .NotEmpty()
                .WithMessage("Session cannot be empty")
            .MaximumLength(100)
                .WithMessage("SessionName cannot exceed 100 characters");
        RuleFor(x => x.MaxParticipants)
            .Must(x => x == -1 || x > 0)
            .WithMessage("MaxParticipants must be -1 (unlimited) or greater than 0");
    }
    
}