using ChatService.Application.Abstractions.Messaging;
using ChatService.Domain.Abstractions;
using ChatService.Domain.Conversations;

namespace ChatService.Application.Conversations.GetConversationByRoomId;

internal sealed class GetConversationByRoomIdQueryHandler : IQueryHandler<GetConversationByRoomIdQuery, ConversationResponse>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    public GetConversationByRoomIdQueryHandler(IConversationRepository conversationRepository, IMessageRepository messageRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    public async Task<Result<ConversationResponse>> Handle(GetConversationByRoomIdQuery request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByRoomId(request.roomId);

        if (conversation is null)
        {
            return Result.Failure<ConversationResponse>(Error.NullValue);
        }

        var messages = await _messageRepository.GetByConversationId(conversation.Id);

        var conversationResponse = new ConversationResponse()
        {
            RoomId = conversation.RoomId,
            Participants = conversation.Participants,
            Messages = messages.Select(message => new MessageResponse()
            {
                SenderId = message.SenderId,
                Content = message.Content,
                CreatedOn = message.CreatedOn
            }).ToList()
        };

        return conversationResponse;
    }
}