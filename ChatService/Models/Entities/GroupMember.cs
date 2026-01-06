namespace ChatService.Models.Entities;

public class GroupMember
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid GroupId { get; set; }
    public GroupChat Group { get; set; } = null!;
}