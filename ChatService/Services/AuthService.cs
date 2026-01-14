using ChatService.Auth;
using ChatService.Data;
using ChatService.Models.DTOs;
using ChatService.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ChatService.Services;
public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _hasher;
    private readonly JwtTokenService _tokenService;

    public AuthService(AppDbContext db, IPasswordHasher<User> hasher, JwtTokenService tokenService)
    {
        _db = db;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    public async Task<object> GetDebugDataAsync()
    {
        var users = await _db.Users
            .ToListAsync();
        var groups = await _db.Groups.Include(c => c.Members).ToListAsync();
        var messages = await _db.Messages.Include(m => m.Sender).ToListAsync();
        return new { users };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            throw new InvalidOperationException("Email already registered");

        var user = new User
        {
            Email = req.Email,
            DisplayName = req.DisplayName,
            AvatarUrl = req.AvatarUrl,
            PasswordHash = _hasher.HashPassword(null!, req.Password)
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _tokenService.CreateToken(user);
        return new AuthResponse(user.Id, user.Email, user.DisplayName, user.AvatarUrl, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == req.Email)
                   ?? throw new Exception("Email not registered, please sign up.");

        var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
        if (verify == PasswordVerificationResult.Failed)
            throw new Exception("Incorrect password, please try again");

        var token = _tokenService.CreateToken(user);
        return new AuthResponse(user.Id, user.Email, user.DisplayName, user.AvatarUrl , token);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest req)
    {
        var user = await _db.Users.FindAsync(userId) ?? throw new KeyNotFoundException("User not found");
        var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.OldPassword);
        if (verify == PasswordVerificationResult.Failed)
            throw new InvalidOperationException("Old password incorrect");

        user.PasswordHash = _hasher.HashPassword(user, req.NewPassword);
        await _db.SaveChangesAsync();
    }

    // Return user without tracking for verification endpoint
    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);
    }

    // Validate ClaimsPrincipal (token) and return user or error
    public async Task<(bool IsValid, string? Error, User? User)> VerifyUserFromPrincipalAsync(ClaimsPrincipal principal)
    {
        var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? principal.FindFirst("sub");
        if (string.IsNullOrEmpty(idClaim?.Value))
            return (false, "Invalid token: missing user ID", null);

        if (!Guid.TryParse(idClaim.Value, out var userId))
            return (false, "Invalid user ID format", null);

        var user = await GetUserByIdAsync(userId);
        if (user == null)
            return (false, "User not found", null);

        return (true, null, user);
    }
}