using Breezy.Application.Users.DTOs;
using Breezy.Domain.Users.ValueObjects;
using MediatR;

namespace Breezy.Application.Users.Commands;

public record UpdateSubscriptionCommand(Guid UserId, SubscriptionPlan NewPlan) : IRequest<UserResponse>;
