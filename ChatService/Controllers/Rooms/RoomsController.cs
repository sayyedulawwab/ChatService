using ChatService.Application.Abstractions.Messaging;
using ChatService.Application.Rooms;
using ChatService.Application.Rooms.CreateRoom;
using ChatService.Application.Rooms.GetRoom;
using ChatService.Application.Rooms.JoinRoom;
using ChatService.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatService.Controllers.Rooms;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RoomsController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id, IQueryHandler<GetRoomQuery, RoomResponse> handler, CancellationToken cancellationToken)
    {
        var query = new GetRoomQuery(id);

        var result = await handler.Handle(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }


    [HttpPost]
    public async Task<IActionResult> Post(CreateRoomRequest request, ICommandHandler<CreateRoomCommand, long> handler, CancellationToken cancellationToken)
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Int64.Parse(userIdClaim.Value);

        var command = new CreateRoomCommand(request.name, request.password, userId);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Created(nameof(Get), new { Id = result.Value });
    }

    [HttpPut]
    public async Task<IActionResult> Put(JoinRoomRequest request, ICommandHandler<JoinRoomCommand, long> handler, CancellationToken cancellationToken)
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Int64.Parse(userIdClaim.Value);

        var command = new JoinRoomCommand(request.roomId, userId);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return NoContent();
    }
}
