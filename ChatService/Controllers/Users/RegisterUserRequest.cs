namespace ChatService.Controllers.Users;

public record RegisterUserRequest(string name, string email, string password);