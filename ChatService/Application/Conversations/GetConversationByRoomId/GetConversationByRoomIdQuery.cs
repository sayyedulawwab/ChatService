using ChatService.Application.Abstractions.Messaging;

namespace ChatService.Application.Conversations.GetConversationByRoomId;

public record GetConversationByRoomIdQuery(long roomId) : IQuery<ConversationResponse>;
