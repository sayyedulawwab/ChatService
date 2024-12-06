namespace ChatService.Domain.Conversations;

public interface IConversationRepository
{
    Task AddAsync(Conversation conversation);
    Task UpdateAsync(Conversation conversation);
    Task RemoveAsync(Conversation conversation);

    Task<Conversation> GetByRoomId(long roomId);

    Task<Conversation> GetByParticipantIds(List<long> participantIds);

    Task AddMessageAsync(Message message);
    Task UpdateMessageAsync(Message message);
    Task RemoveMessageAsync(Message message);
}