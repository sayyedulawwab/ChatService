using ChatService.Application.Abstractions.Messaging;
using ChatService.Domain.Abstractions;
using ChatService.Domain.Conversations;

namespace ChatService.Application.Conversations.GetMessagesByConversationId;

internal sealed class GetMessagesByConversationIdQueryHandler : IQueryHandler<GetMessagesByConversationIdQuery, List<MessageResponse>>
{
    private readonly IMessageRepository _messageRepository;
    public GetMessagesByConversationIdQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<Result<List<MessageResponse>>> Handle(GetMessagesByConversationIdQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetByConversationIdAsync(request.conversationId, 0, 50);

        if (messages is null)
        {
            return Result.Failure<List<MessageResponse>>(Error.NullValue);
        }

        var conversationResponse = messages.Select(message =>  new MessageResponse()
        {
            SenderName = message.SenderName,
            Content = message.Content,
            CreatedOn = message.CreatedOn

        }).ToList();

        return conversationResponse;
    }
}