namespace ChatService.Domain.Entities;

public class Room
{
    public Room(string name, string passwordHash, int createdBy, DateTime createdOn)
    {
        Name = name;
        PasswordHash = passwordHash;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string PasswordHash { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime CreatedOn { get; private set; }


   public static Room Create(string name, string passwordHash, int createdBy, DateTime createdOn)
   {
        var room = new Room(name, passwordHash, createdBy, createdOn);
        
        return room;
   }

}