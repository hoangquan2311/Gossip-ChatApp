using System.ComponentModel.DataAnnotations;

namespace ChatService.Models.Entities;

public class User
{
    // Primary Key - Global Unique Identifier for each user
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required, MaxLength(200)]
    public string DisplayName { get; set; } = null!;
    [Required]
    public string PasswordHash { get; set; } = null!;
    public string AvatarUrl { get; set; } = string.Empty;
    public ICollection<GroupMember> Groups { get; set; } = new List<GroupMember>();
}