using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatService.Domain.Conversations;

public class Conversation
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; }
    public long? RoomId { get; set; }
    public List<long> Participants { get; set; }
    public bool IsGroup { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}
