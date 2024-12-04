namespace ChatService.Domain.Rooms;

public class Room
{
    public Room(string name, string passwordHash, long createdBy, DateTime createdOn)
    {
        Name = name;
        PasswordHash = passwordHash;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
    }

    public long Id { get; private set; }
    public string Name { get; private set; }
    public string PasswordHash { get; private set; }
    public long CreatedBy { get; private set; }
    public DateTime CreatedOn { get; private set; }


    public static Room Create(string name, string passwordHash, long createdBy, DateTime createdOn)
    {
        var room = new Room(name, passwordHash, createdBy, createdOn);

        return room;
    }

}