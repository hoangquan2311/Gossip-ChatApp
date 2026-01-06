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
        return new MessageDto(msg.Id, msg.GroupId, senderId, sender.DisplayName, msg.Content, msg.SentAt, Enumerable.Empty<MessageReaderDto>());
    }

    public async Task<IEnumerable<MessageDto>> GetHistoryAsync(Guid groupId, int take = 20, Guid? beforeMessageId = null)
    {
        var query = _db.Messages
        .Include(m => m.Sender)
        .Include(m => m.Readers).ThenInclude(r => r.User)
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
        .Take(take)
        .OrderBy(m => m.SentAt)
        .ToListAsync();

        return data.Select(m => new MessageDto(
            m.Id,
            m.GroupId,
            m.SenderId,
            m.Sender.DisplayName,
            m.Content,
            m.SentAt,
            m.Readers.Select(r => new MessageReaderDto(r.UserId, r.User.DisplayName))
        ));
    }
    // Mark a message as readed by an user
    public async Task MarkAsReadedAsync(Guid messageId, Guid userId)
       {
           var existingReader = await _db.Readers
               .FirstOrDefaultAsync(mv => mv.MessageId == messageId && mv.UserId == userId);
           if (existingReader == null)
           {
               _db.Readers.Add(new MessageReader { MessageId = messageId, UserId = userId });
               await _db.SaveChangesAsync();
           }
       }

       // Get the list of users who have readed a specific message
       public async Task<IEnumerable<MessageReaderDto>> GetReadersAsync(Guid messageId)
       {
           return await _db.Readers
               .Where(mv => mv.MessageId == messageId)
               .Include(mv => mv.User)
               .Select(mv => new MessageReaderDto(mv.UserId, mv.User.DisplayName))
               .ToListAsync();
       }
}