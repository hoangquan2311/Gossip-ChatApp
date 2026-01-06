using ChatService.Data;
using ChatService.Models.DTOs;
using ChatService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Services;

public class GroupService
{
    private readonly AppDbContext _db;

    public GroupService(AppDbContext db) => _db = db;

    public async Task<GroupDto> CreateAsync(Guid creatorId, CreateGroupRequest req)
    {
        var participants = req.ParticipantIds.Distinct().ToList();
        if (!participants.Contains(creatorId))
            participants.Add(creatorId);

        var group = new GroupChat
        {
            Title = req.Title
        };
        _db.Groups.Add(group);

        foreach (var pid in participants)
        {
            _db.GroupMembers.Add(new GroupMember
            {
                Group = group,
                UserId = pid
            });
        }

        await _db.SaveChangesAsync();
        return await MapGroup(group.Id);
    }

    public async Task<IEnumerable<GroupDto>> GetForUserAsync(Guid userId)
    {
        var convIds = await _db.GroupMembers
            .Where(gm => gm.UserId == userId)
            .Select(gm => gm.GroupId)
            .ToListAsync();

        var list = new List<GroupDto>();
        foreach (var id in convIds)
            list.Add(await MapGroup(id));

        return list;
    }
    // Helper method to map GroupChat entity to GroupDto
    public async Task<GroupDto> MapGroup(Guid id)
    {
        var group = await _db.Groups
            .Include(c => c.Members).ThenInclude(m => m.User)
            .SingleAsync(c => c.Id == id);

        var members = group.Members
            .Select(m => new MemberDto(m.UserId, m.User.DisplayName, m.User.Email));

        return new GroupDto(group.Id, group.Title!, members);
    }

    public async Task<bool> UserInGroup(Guid userId, Guid groupId) =>
        await _db.GroupMembers.AnyAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
}