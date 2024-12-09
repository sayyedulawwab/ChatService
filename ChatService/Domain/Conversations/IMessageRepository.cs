namespace ChatService.Domain.Conversations;

public interface IMessageRepository
{
    Task<List<Message>> GetByConversationIdAsync(string conversationId, int skip, int limit);
    Task AddAsync(Message message);
    Task UpdateAsync(string id, Message message);
    Task RemoveAsync(string id);
}