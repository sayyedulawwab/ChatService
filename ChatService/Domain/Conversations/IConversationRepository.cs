namespace ChatService.Domain.Conversations;

public interface IConversationRepository
{
    Task AddAsync(Conversation conversation);
    Task UpdateAsync(string id, Conversation conversation);
    Task RemoveAsync(string id);
    Task<Conversation> GetByRoomId(long roomId);
    Task<Conversation> GetByParticipantIds(List<long> participantIds);
}