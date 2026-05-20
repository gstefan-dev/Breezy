using Breezy.Application.Interfaces;
using Breezy.Application.Users.DTOs;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Users.Queries;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserQueryHandler> _logger;

    public GetUserQueryHandler(IUserRepository userRepository, ILogger<GetUserQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserResponse> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user {UserId}", query.UserId);

        var user = await _userRepository.GetUserByIdAsync(query.UserId);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", query.UserId);
            throw new DomainException("User not found");
        }
        
        _logger.LogInformation("User {UserId} retrieved successfully", query.UserId);
        return new UserResponse(user.Id, user.Email.ToString(), user.UserName,
            user.Role.ToString(), user.Plan.ToString(), user.CreatedAt);
    }
}