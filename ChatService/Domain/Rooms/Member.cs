using ChatService.Domain.Users;

namespace ChatService.Domain.Rooms;

public class Member
{
    private Member(long userId, long roomId, Role role, DateTime joinedOn)
    {
        UserId = userId;
        RoomId = roomId;
        Role = role;
        JoinedOn = joinedOn;
    }

    private Member()
    {
    }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public User User { get; private set; }  // Navigation property
    public long RoomId { get; private set; }
    public Role Role { get; private set; }
    public DateTime JoinedOn { get; private set; }

    public static Member Create(long userId, long roomId, Role role, DateTime joinedOn)
    {
        var member = new Member(userId, roomId, role, joinedOn);

        return member;
    }
}