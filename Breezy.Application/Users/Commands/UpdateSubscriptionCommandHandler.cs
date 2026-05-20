using Breezy.Application.Interfaces;
using Breezy.Application.Users.DTOs;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Users.Commands;

public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateSubscriptionCommandHandler> _logger;

    public UpdateSubscriptionCommandHandler(IUserRepository userRepository, ILogger<UpdateSubscriptionCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserResponse> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating subscription for UserId: {UserId}", request.UserId);

        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", request.UserId);
            throw new DomainException("User doesnt exist");
        }

        user.UpdateSubscription(request.NewPlan);

        await _userRepository.UpdateAsync(user);
        
        _logger.LogInformation("Subscription updated successfully for UserId: {UserId}", request.UserId);
        
        return new UserResponse(user.Id, user.Email.ToString(), user.UserName, user.Role.ToString(), request.NewPlan.ToString(),
            user.CreatedAt);
    }
}