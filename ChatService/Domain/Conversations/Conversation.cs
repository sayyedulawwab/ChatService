using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatService.Domain.Conversations;

public class Conversation
{
    private Conversation(long? roomId, List<long> participants, bool isGroup, DateTime createdOn)
    {
        RoomId = roomId;
        Participants = participants;
        IsGroup = isGroup;
        CreatedOn = createdOn;
    }


    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; }
    public long? RoomId { get; private set; } // null for private conversation
    public List<long> Participants { get; private set; } // empty for room conversation. userid for user to user chat
    public bool IsGroup { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime UpdatedOn { get; private set; }

    public static Conversation Create(long? roomId, List<long> participants, bool isGroup, DateTime createdOn)
    {
        if (isGroup)
        {
            if (roomId == null || participants.Any())
                throw new ArgumentException(ConversationErrors.InvalidGroupConversation.description);
        }
        else
        {
            if (roomId != null || participants.Count != 2)
                throw new ArgumentException(ConversationErrors.InvalidDirectConversation.description);
        }

        var conversation = new Conversation(roomId, participants, isGroup, createdOn);

        return conversation;
    }
}