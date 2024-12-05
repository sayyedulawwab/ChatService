using ChatService.Domain.Users;
using ChatService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Infrastructure.Repositories;
internal sealed class UserRepository : IUserRepository
{
    private readonly EfDbContext _dbContext;

    public UserRepository(EfDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(User user)
    {
        _dbContext.Set<User>().Add(user);
    }

    public async Task<User?> GetById(long id)
    {
        var user = await _dbContext.Set<User>().FindAsync(id);

        return user;
    }

    public async Task<User?> GetByEmail(string email)
    {
        var user = await _dbContext.Set<User>().Where(user => user.Email == email).FirstOrDefaultAsync();

        return user;
    }

    public void Remove(User user)
    {
        _dbContext.Set<User>().Remove(user);
    }

    public void Update(User user)
    {
        _dbContext.Set<User>().Update(user);
    }
}
