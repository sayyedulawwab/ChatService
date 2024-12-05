namespace ChatService.Domain.Rooms;

public interface IRoomRepository
{
    Task<Room?> GetById(long id);
    Task<Room?> GetByIdWitMembers(long id);
    void Add(Room room);
    void Update(Room room);
    void Remove(Room room);
}