using System.ComponentModel.DataAnnotations;

namespace ChatService.Models.Entities;

public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid GroupId { get; set; }
    public GroupChat Group { get; set; } = null!;

    public Guid SenderId { get; set; }
    public User Sender { get; set; } = null!;

    [Required, MaxLength(2000)]
    public string Content { get; set; } = null!;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public ICollection<MessageReader> Readers { get; set; } = new List<MessageReader>();
}