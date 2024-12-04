namespace ChatService.Domain.Conversations;

public interface IConversationRepository
{
    void Add(Conversation conversation);
    void Update(Conversation conversation);
    void Remove(Conversation conversation);
}
