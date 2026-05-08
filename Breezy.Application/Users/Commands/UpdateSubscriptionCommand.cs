using Breezy.Domain.Users.ValueObjects;

namespace Breezy.Application.Users.Commands;

public record UpdateSubscriptionCommand(Guid UserId, SubscriptionPlan NewPlan);
