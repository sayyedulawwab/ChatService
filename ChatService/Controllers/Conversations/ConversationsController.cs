using ChatService.Application.Conversations.CreateConversation;
using ChatService.Application.Conversations.GetConversationByRoomId;
using ChatService.Application.Conversations.GetMessagesByConversationId;
using ChatService.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers.Conversations;
[Route("api/conversations")]
[ApiController]
[Authorize]
public class ConversationsController : ControllerBase
{
    private readonly ISender _sender;

    public ConversationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateConversation(CreateConversationRequest request)
    {
        var command = new CreateConversationCommand(
            request.roomId,
            request.participants);

        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }
    
    [HttpGet("{roomId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetConversation(long roomId, [FromQuery] int page, [FromQuery] int pageSize)
    {
        var query = new GetConversationByRoomIdQuery(roomId, page, pageSize);

        var result = await _sender.Send(query);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }

    [HttpGet("history/{roomId}")]
    public async Task<IActionResult> GetChatHistory(string roomId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetMessagesByConversationIdQuery(roomId);

        var result = await _sender.Send(query);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }
}