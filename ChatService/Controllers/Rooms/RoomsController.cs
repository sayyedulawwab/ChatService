using ChatService.Application.Rooms.CreateRoom;
using ChatService.Application.Rooms.GetRoom;
using ChatService.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatService.Controllers.Rooms;
[Route("api/[controller]")]
[ApiController]
public class RoomsController : ControllerBase
{
    private readonly ISender _sender;

    public RoomsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var query = new GetRoomQuery(id);

        var result = await _sender.Send(query);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }


    [HttpPost]
    public async Task<IActionResult> Post(CreateRoomRequest request)
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Int64.Parse(userIdClaim.Value);

        var command = new CreateRoomCommand(request.name, request.password, userId);

        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Created(nameof(Get), new { Id = result.Value });
    }
}
