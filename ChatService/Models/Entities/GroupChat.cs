using System.ComponentModel.DataAnnotations;

namespace ChatService.Models.Entities;

public class GroupChat
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(200)]
    public string? Title { get; set; }

    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}