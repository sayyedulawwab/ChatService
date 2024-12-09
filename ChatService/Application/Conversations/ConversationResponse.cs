namespace ChatService.Application.Conversations;

public class ConversationResponse
{
    public long? RoomId { get; init; }
     public List<long> Participants { get; init; }
    public List<MessageResponse> Messages { get; init; }
}

public class MessageResponse
{
    public string SenderName { get; init; }
    public string Content { get; init; }
    public DateTime CreatedOn { get; init; }
}