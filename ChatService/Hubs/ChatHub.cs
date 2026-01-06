using ChatService.Models.DTOs;
using ChatService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatService.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly GroupService _groups;
    private readonly MessageService _messages;

    public ChatHub(GroupService groups, MessageService messages)
    {
        _groups = groups;
        _messages = messages;
    }

    public async Task JoinGroup(Guid groupId)
    {
        var userId = GetUserId();
        if (!await _groups.UserInGroup(userId, groupId))
            throw new HubException("Not a member of this group");

        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
    }

    public async Task SendMessage(SendMessageRequest req)
    {
        var userId = GetUserId();
        if (!await _groups.UserInGroup(userId, req.GroupId))
            throw new HubException("Not a member of this group");

        var saved = await _messages.SaveAsync(userId, req);
        await Clients.Group(req.GroupId.ToString())
            .SendAsync("ReceiveMessage", saved);
    }

    private Guid GetUserId()
    {
        var sub = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new HubException("Unauthorized");
        return Guid.Parse(sub);
    }
}