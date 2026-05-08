using Breezy.Application.Interfaces;
using Breezy.Application.Users.DTOs;
using Breezy.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Users.Commands;

public class UpdateSubscriptionCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateSubscriptionCommandHandler> _logger;

    public UpdateSubscriptionCommandHandler(IUserRepository userRepository, ILogger<UpdateSubscriptionCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserResponse> Handle(UpdateSubscriptionCommand command)
    {
        _logger.LogInformation("Updating subscription for UserId: {UserId}", command.UserId);

        var user = await _userRepository.GetUserByIdAsync(command.UserId);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            throw new DomainException("User doesnt exist");
        }

        user.UpdateSubscription(command.NewPlan);

        await _userRepository.UpdateAsync(user);
        
        _logger.LogInformation("Subscription updated successfully for UserId: {UserId}", command.UserId);
        
        return new UserResponse(user.Id, user.Email.ToString(), user.UserName, user.Role.ToString(), command.NewPlan.ToString(),
            user.CreatedAt);
    }
}