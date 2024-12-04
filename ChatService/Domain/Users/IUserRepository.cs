namespace ChatService.Domain.Users;

public interface IUserRepository
{
    void Add(User user);
    void Update(User user);
    void Remove(User user);
}