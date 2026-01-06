using ChatService.Models.DTOs;
using ChatService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatService.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly GroupService _groups;
    private readonly MessageService _messages;

    public MessagesController(GroupService groups, MessageService messages)
    {
        _groups = groups;
        _messages = messages;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> Send(SendMessageRequest req)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (!await _groups.UserInGroup(userId, req.GroupId))
            return Forbid();

        var saved = await _messages.SaveAsync(userId, req);
        return Ok(saved);
    }
}