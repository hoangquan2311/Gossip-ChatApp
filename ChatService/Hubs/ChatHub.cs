using ChatService.Models.DTOs;
using ChatService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace ChatService.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly GroupService _groups;
    private readonly MessageService _messages;
    private static ConcurrentDictionary<string, string> _users =
            new ConcurrentDictionary<string, string>();

    public ChatHub(GroupService groups, MessageService messages)
    {
        _groups = groups;
        _messages = messages;
    }

    /// Gọi khi client kết nối với hub
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    /// Gọi khi client ngắt kết nối
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        
        var userId = _users.FirstOrDefault(u => u.Value == Context.ConnectionId).Key;

        if (!string.IsNullOrEmpty(userId))
        {
            _users.TryRemove(userId, out _);

            //// Thông báo cho tất cả client rằng user đã disconnect
            //await Clients.All.SendAsync("UserDisconnected", userName);

            //// Cập nhật danh sách user
            //await Clients.All.SendAsync("ReceiveUserList", _users.Keys.ToList());

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] User disconnected: {userId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// Xử lý khi user mới kết nối và gửi tên của họ
    /// Được gọi từ client bằng: signalRService.invoke("SendUserConnected", userName)
    public async Task SendUserConnected(string userId)
    {
        // Thêm user vào danh sách
        _users.TryAdd(userId, Context.ConnectionId);

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] User connected: {userId} ({Context.ConnectionId})");
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Total users: {_users.Count}");

        // Gửi danh sách user cập nhật cho tất cả client
        //await Clients.All.SendAsync("ReceiveUserList", _users.Keys.ToList());

        //// Thông báo có user mới đến tất cả client
        //await Clients.All.SendAsync("UserConnected", userName);
    }

    /// Xử lý khi user rời khỏi
    /// Được gọi từ client bằng: signalRService.invoke("SendUserDisconnected", userName)
    public async Task SendUserDisconnected(string userId)
    {
        _users.TryRemove(userId, out _);

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] User left: {userId}");
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Total users: {_users.Count}");
       
        // Thông báo user đã rời
        //await Clients.All.SendAsync("UserDisconnected", userName);
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

    // Create a group from client request and return the created group dto
    [HubMethodName("createGroup")]
    public async Task<GroupDto> CreateGroup(CreateGroupRequest req)
    {
        var userId = GetUserId();
        var group = await _groups.CreateAsync(userId, req);
        // Add caller to the SignalR group so they start receiving messages
        await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
        return group;
    }

    private Guid GetUserId()
    {
        var sub = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new HubException("Unauthorized");
        return Guid.Parse(sub);
    }
}