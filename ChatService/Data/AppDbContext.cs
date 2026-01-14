using ChatService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<GroupChat> Groups => Set<GroupChat>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        b.Entity<GroupMember>()
            .HasKey(gm => new { gm.UserId, gm.GroupId });

        b.Entity<GroupMember>()
            .HasOne(gm => gm.User)
            .WithMany(u => u.Groups)
            .HasForeignKey(gm => gm.UserId);

        b.Entity<GroupMember>()
            .HasOne(gm => gm.Group)
            .WithMany(c => c.Members)
            .HasForeignKey(gm => gm.GroupId);

        b.Entity<Message>()
            .HasOne(m => m.Group)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.GroupId);

        b.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId);
    }
}