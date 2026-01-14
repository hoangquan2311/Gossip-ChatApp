using ChatService.Models.DTOs;
using ChatService.Data;
using ChatService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Services;

public class MessageService
{
    private readonly AppDbContext _db;

    public MessageService(AppDbContext db) => _db = db;

    // 
    public async Task<MessageDto> SaveAsync(Guid senderId, SendMessageRequest req)
    {
        var msg = new Message
        {
            GroupId = req.GroupId,
            SenderId = senderId,
            Content = req.Content
        };
        _db.Messages.Add(msg);
        await _db.SaveChangesAsync();

        var sender = await _db.Users.FindAsync(senderId) ?? throw new KeyNotFoundException("Sender not found");
        return new MessageDto(msg.Id, msg.GroupId, senderId, sender.DisplayName, msg.Content, msg.SentAt);
    }

    public async Task<IEnumerable<MessageDto>> GetHistoryAsync(Guid groupId, Guid? beforeMessageId = null)
    {
        var query = _db.Messages
        .Include(m => m.Sender)
        .Where(m => m.GroupId == groupId);

        if (beforeMessageId.HasValue)
        {
            // Lấy SentAt của message trước đó để lọc chính xác
            var beforeMessage = await _db.Messages.FindAsync(beforeMessageId.Value);
            if (beforeMessage != null)
            {
                query = query.Where(m => m.SentAt < beforeMessage.SentAt || (m.SentAt == beforeMessage.SentAt && m.Id != beforeMessageId.Value));
            }
        }

        var data = await query
        .OrderByDescending(m => m.SentAt)
        .OrderBy(m => m.SentAt)
        .ToListAsync();

        return data.Select(m => new MessageDto(
            m.Id,
            m.GroupId,
            m.SenderId,
            m.Sender.DisplayName,
            m.Content,
            m.SentAt
        ));
    }
}