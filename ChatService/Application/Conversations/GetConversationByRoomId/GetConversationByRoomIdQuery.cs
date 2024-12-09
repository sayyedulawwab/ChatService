using ChatService.Application.Abstractions.Messaging;

namespace ChatService.Application.Conversations.GetConversationByRoomId;

public record GetConversationByRoomIdQuery(long roomId, int page, int pageSize) : IQuery<ConversationResponse>;
