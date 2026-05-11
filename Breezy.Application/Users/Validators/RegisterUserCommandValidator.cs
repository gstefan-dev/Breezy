using Breezy.Application.Users.Commands;
using FluentValidation;

namespace Breezy.Application.Users.Validators;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("UserName cannot be empty")
            .MaximumLength(50)
            .WithMessage("UserName cannot exceed 50 characters");
        RuleFor(x => x.UserEmail)
            .NotEmpty()
            .WithMessage("UserEmail cannot be empty")
            .EmailAddress()
            .WithMessage("Invalid email format");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password cannot be empty")
            .MinimumLength(8)
            .WithMessage("Minimum length needs be minimum 8 characters")
            .MaximumLength(100)
            .WithMessage("Maximum lengths needs be maximum 100 characters");
        
    }
}