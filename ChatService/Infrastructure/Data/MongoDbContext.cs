using ChatService.Domain.Conversations;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
        CreateIndexes();
    }

    public IMongoCollection<Conversation> Conversations => _database.GetCollection<Conversation>("Conversations");
    public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");

    public IMongoCollection<TEntity> GetCollection<TEntity>(string collectionName)
    {
        return _database.GetCollection<TEntity>(collectionName);
    }

    private void CreateIndexes()
    {
        var conversationIndex = Builders<Conversation>.IndexKeys.Ascending(c => c.RoomId);
        Conversations.Indexes.CreateOne(new CreateIndexModel<Conversation>(conversationIndex));

        var messageIndex = Builders<Message>.IndexKeys.Ascending(m => m.ConversationId);
        Messages.Indexes.CreateOne(new CreateIndexModel<Message>(messageIndex));
    }
}