using ChatService.Domain.Rooms;
using ChatService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Infrastructure.Repositories;

internal sealed class RoomRepository : IRoomRepository
{
    private readonly EfDbContext _dbContext;
    public RoomRepository(EfDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Room room)
    {
        _dbContext.Set<Room>().Add(room);
    }

    public async Task<Room?> GetById(long id)
    {
        var room = await _dbContext.Set<Room>().FindAsync(id);

        return room;
    }

    public async Task<Room?> GetByIdWithMembers(long id)
    {
        var room = await _dbContext.Set<Room>()
            .Include(room => room.Members)
            .ThenInclude(member => member.User)
            .FirstOrDefaultAsync(room => room.Id == id);

        return room;
    }

    public void Remove(Room room)
    {
        _dbContext?.Set<Room>().Remove(room);
    }

    public void Update(Room room)
    {
        _dbContext.Set<Room>().Update(room);
    }
}