using ChatService.Auth;
using ChatService.Data;
using ChatService.Models.DTOs;
using ChatService.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            PasswordHash = _hasher.HashPassword(null!, req.Password)
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _tokenService.CreateToken(user);
        return new AuthResponse(user.Id, user.Email, user.DisplayName, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == req.Email)
                   ?? throw new UnauthorizedAccessException("Email not registered");

        var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
        if (verify == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Incorrect password");

        var token = _tokenService.CreateToken(user);
        return new AuthResponse(user.Id, user.Email, user.DisplayName, token);
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
}