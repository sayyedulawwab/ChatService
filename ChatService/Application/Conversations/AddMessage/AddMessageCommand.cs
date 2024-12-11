using ChatService.Application.Abstractions.Messaging;

namespace ChatService.Application.Conversations.CreateConversation;

public record AddMessageCommand(long roomId, long senderId, string content) : ICommand<string>;