using ChatService.Application.Abstractions.Messaging;

namespace ChatService.Application.Conversations.GetMessagesByConversationId;

public record GetMessagesByConversationIdQuery(string conversationId) : IQuery<List<MessageResponse>>;
