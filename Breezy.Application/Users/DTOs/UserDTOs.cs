namespace Breezy.Application.Users.DTOs;

public record UserResponse(Guid Id, string Email, string UserName, string Role, string Plan, DateTime CreatedAt);