using ChatService.Domain.Conversations;
using ChatService.Infrastructure.Data;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly IMongoCollection<Message> MessageCollection;

    public MessageRepository(MongoDbContext context)
    {
        MessageCollection = context.GetCollection<Message>("Messages");
    }

    public async Task AddAsync(Message message)
    {
        await MessageCollection.InsertOneAsync(message, null);
    }

    public async Task RemoveAsync(string id)
    {
        var filter = Builders<Message>.Filter.Eq(e => e.Id, id);
        await MessageCollection.DeleteOneAsync(filter);
    }

    public async Task UpdateAsync(string id, Message message)
    {
        var filter = Builders<Message>.Filter.Eq(e => e.Id, id);
        await MessageCollection.ReplaceOneAsync(filter, message, new ReplaceOptions());
    }
    public async Task<List<Message>> GetByConversationIdAsync(string conversationId, int skip, int limit)
    {
        var filter = Builders<Message>.Filter.Eq(m => m.ConversationId, conversationId);
        return await MessageCollection.Find(filter)
                                      .SortBy(m => m.CreatedOn)
                                      .Skip(skip)
                                      .Limit(limit)
                                      .ToListAsync();
    }
}
