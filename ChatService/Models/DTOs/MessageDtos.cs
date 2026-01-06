namespace ChatService.Models.DTOs;

public record SendMessageRequest(Guid GroupId, string Content);
public record MessageDto(Guid Id, Guid GroupId, Guid SenderId, string SenderName, string Content, DateTime SentAt, IEnumerable<MessageReaderDto> Readers);
public record MessageReaderDto(Guid UserId, string UserName);