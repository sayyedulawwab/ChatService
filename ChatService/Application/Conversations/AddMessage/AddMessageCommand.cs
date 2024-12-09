using ChatService.Application.Abstractions.Messaging;

namespace ChatService.Application.Conversations.CreateConversation;

public record AddMessageCommand(string conversationId, long senderId, string content) : ICommand<string>;