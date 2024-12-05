using ChatService.Domain.Users;

namespace ChatService.Domain.Rooms;

public class Member
{
    private Member(long userId, long roomId, DateTime joinedOn)
    {
        UserId = userId;
        RoomId = roomId;
        JoinedOn = joinedOn;
    }

    private Member()
    {
    }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public long RoomId { get; private set; }
    public Role Role { get; private set; }
    public DateTime JoinedOn { get; private set; }
    public User User { get; private set; }  // Navigation property
    public Room Room { get; private set; } // Navigation property

    public static Member Create(long userId, long roomId, DateTime joinedOn)
    {
        var member = new Member(userId, roomId, joinedOn);

        return member;
    }
}