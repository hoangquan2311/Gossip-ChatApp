using System.Security.Claims;
using ChatService.Models.DTOs;
using ChatService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
    {
        try
        {
            var res = await _auth.RegisterAsync(req);
            return Ok(res);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("verify-me")]
    [Authorize]
    public async Task<IActionResult> VerifyMe()
    {
        var (IsValid, Error, UserFound) = await _auth.VerifyUserFromPrincipalAsync(User);
        if (!IsValid)
        {
            if (Error == "User not found")
                return BadRequest(Error);
            return BadRequest(Error);
        }
        var res = new
        {
            UserId = UserFound!.Id,
            UserFound.Email,
            UserFound.DisplayName,
            UserFound.AvatarUrl
        };

        return Ok(res);
    }

    [HttpGet("debug-data")]  // Temporary endpoint; remove after debugging
    public async Task<IActionResult> DebugData()
    {
        var res = await _auth.GetDebugDataAsync();

        return Ok(res);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        try
        {
            var res = await _auth.LoginAsync(req);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
    {
        try
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst("sub");
            if (string.IsNullOrEmpty(idClaim?.Value))
                return BadRequest("Invalid token: missing user ID");

            if (!Guid.TryParse(idClaim.Value, out var userId))
                return BadRequest ("Invalid user ID format");

            await _auth.ChangePasswordAsync(userId, req);
            return Ok("Password changed successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}