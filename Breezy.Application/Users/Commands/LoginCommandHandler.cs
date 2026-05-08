using Breezy.Application.Interfaces;
using Breezy.Application.Users.DTOs;
using Breezy.Domain.Shared;
using Breezy.Domain.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Users.Commands;

public class LoginCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IUserRepository userRepository, IAuthService authService, ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _authService = authService;
        _logger = logger;
    }

    public async Task<string> Handle(LoginCommand command)
    {
        _logger.LogInformation("Login attempt for {Email}", command.Email);

        var email = Email.From(command.Email);

        var existingUser = await _userRepository.GetUserByEmailAsync(email);
        if (existingUser == null)
        {
            _logger.LogWarning("User with email {Email} not found", command.Email);
            throw new DomainException("Invalid credentials");

        }

        var password = await _authService.VerifyPasswordAsync(command.Password, existingUser.PasswordHash);

        if (password == false)
        {
            _logger.LogWarning("Password is wrong");
            throw new DomainException("Invalid credentials");

        }

        var jwtToken = await _authService.GenerateTokenAsync(existingUser);
        _logger.LogInformation("User {Email} logged in successfully", command.Email);

        return jwtToken;
    }
}