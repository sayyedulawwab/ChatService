using ChatService.Application.Abstractions.Messaging;

namespace ChatService.Application.Users.Login;

public record LoginUserQuery(string email, string password) : IQuery<AccessTokenResponse>;