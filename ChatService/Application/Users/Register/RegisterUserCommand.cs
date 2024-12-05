using ChatService.Application.Abstractions.Messaging;

namespace ChatService.Application.Users.Register;

public record RegisterUserCommand(string name, string email, string password) : ICommand<long>;