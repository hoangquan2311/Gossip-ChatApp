namespace ChatService.Models.DTOs;

public record GroupDto(Guid Id, string Title, IEnumerable<MemberDto> Members);
public record MemberDto(Guid UserId, string DisplayName, string Email);
public record CreateGroupRequest(string Title, IEnumerable<Guid> ParticipantIds);