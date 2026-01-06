using ChatService.Models.DTOs;
using ChatService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatService.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly GroupService _groups;
    private readonly MessageService _messages;

    public GroupsController(GroupService groups, MessageService messages)
    {
        _groups = groups;
        _messages = messages;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetMyGroups()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var res = await _groups.GetForUserAsync(userId);
        return Ok(res);
    }

    [HttpPost]
    public async Task<ActionResult<GroupDto>> Create(CreateGroupRequest req)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var res = await _groups.CreateAsync(userId, req);
        return Ok(res);
    }

    [HttpGet("{groupId:guid}/messages")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> History(Guid groupId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (!await _groups.UserInGroup(userId, groupId))
            return Forbid();

        var res = await _messages.GetHistoryAsync(groupId);
        return Ok(res);
    }
}