namespace ChatService.Models.DTOs;

public record RegisterRequest(string Email, string DisplayName, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(Guid UserId, string Email, string DisplayName, string Token);
public record ChangePasswordRequest(string OldPassword, string NewPassword);