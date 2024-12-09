using ChatService.Domain.Abstractions;

namespace ChatService.Application.Abstractions.Auth;
public interface IJwtService
{
    Result<string> GetAccessToken(string name, string email, long userId);
}
