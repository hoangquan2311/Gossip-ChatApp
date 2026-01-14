namespace ChatService.Models.DTOs;

public record GroupDto(Guid Id, string Title, string AvatarUrl, IEnumerable<MemberDto> Members);
public record MemberDto(Guid UserId, string DisplayName, string Email, string AvatarUrl);
public record CreateGroupRequest(string Title, IEnumerable<Guid> ParticipantIds);