namespace ChatService.Domain.Conversations;

public interface IMessageRepository
{
    Task<List<Message>> GetByConversationId(string conversationId);
}
