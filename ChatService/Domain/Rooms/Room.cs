namespace ChatService.Domain.Rooms;

public class Room
{
    public Room(string name, string passwordHash, DateTime createdOn)
    {
        Name = name;
        PasswordHash = passwordHash;
        CreatedOn = createdOn;
    }

    public long Id { get; private set; }
    public string Name { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public List<Member> Members { get; private set; } = new List<Member>();


    public static Room Create(string name, string passwordHash, DateTime createdOn)
    {
        var room = new Room(name, passwordHash, createdOn);

        return room;
    }

    public void AddMember(long userId, long roomId, DateTime joinedOn)
    {
        var member = Member.Create(userId, roomId, joinedOn);

        Members.Add(member);
    }

}