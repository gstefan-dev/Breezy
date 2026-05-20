using Breezy.Application.Interfaces;
using Breezy.Application.Users.DTOs;
using Breezy.Domain.Shared;
using Breezy.Domain.Users.Entities;
using Breezy.Domain.Users.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Users.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    private readonly IAuthService _authService;

    public RegisterUserCommandHandler(IUserRepository userRepository, ILogger<RegisterUserCommandHandler> logger,
        IAuthService authService)
    {
        _userRepository = userRepository;
        _logger = logger;
        _authService = authService;
    }

    public async Task<UserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering user {UserName}", request.UserName);
        var email = Email.From(request.UserEmail);
        var existingUser = await _userRepository.GetUserByEmailAsync(email);
        if (existingUser != null)
        {
            _logger.LogWarning("Email {Email} already in use", request.UserEmail);
            throw new DomainException("Email already in use");
        }

        var passwordHash = await _authService.HashPasswordAsync(request.Password);

        var user = User.Create(email, passwordHash, request.UserName);

        await _userRepository.AddAsync(user);
        
        _logger.LogInformation("User {UserName} registered successfully", request.UserName);
        return new UserResponse(user.Id, user.Email.ToString(), user.UserName, user.Role.ToString(), user.Plan.ToString(),
            user.CreatedAt);
    }
}