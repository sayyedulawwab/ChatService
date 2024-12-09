using ChatService.Domain.Conversations;
using ChatService.Infrastructure.Data;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly IMongoCollection<Conversation> ConversationCollection;

    public ConversationRepository(MongoDbContext context)
    {
        ConversationCollection = context.GetCollection<Conversation>("Conversations");
    }

    public async Task AddAsync(Conversation conversation)
    {
        await ConversationCollection.InsertOneAsync(conversation, null);
    }

    public async Task RemoveAsync(string id)
    {
        var filter = Builders<Conversation>.Filter.Eq(e => e.Id, id);
        await ConversationCollection.DeleteOneAsync(filter);
    }

    public async Task UpdateAsync(string id, Conversation conversation)
    {
        var filter = Builders<Conversation>.Filter.Eq(e => e.Id, id);
        await ConversationCollection.ReplaceOneAsync(filter, conversation, new ReplaceOptions());
    }

    public async Task<Conversation> GetByRoomId(long roomId)
    {
        var filter = Builders<Conversation>.Filter.Eq(e => e.RoomId, roomId);
        return await ConversationCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Conversation> GetByParticipantIds(List<long> participantIds)
    {
        var filter = Builders<Conversation>.Filter.All(e => e.Participants, participantIds);
        return await ConversationCollection.Find(filter).FirstOrDefaultAsync();
    }


}