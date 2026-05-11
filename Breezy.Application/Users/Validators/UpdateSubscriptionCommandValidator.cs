using Breezy.Application.Users.Commands;
using FluentValidation;

namespace Breezy.Application.Users.Validators;

public class UpdateSubscriptionCommandValidator : AbstractValidator<UpdateSubscriptionCommand>
{
    public UpdateSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty");
        RuleFor(x => x.NewPlan)
            .IsInEnum()
            .WithMessage("Invalid subscription plan");

    }
}