namespace ChatService.Domain.Rooms;

public interface IRoomRepository
{
    void Add(Room room);
    void Update(Room room);
    void Remove(Room room);
}