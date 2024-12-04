using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers.Rooms;
[Route("api/[controller]")]
[ApiController]
public class RoomsController : ControllerBase
{
    public RoomsController()
    {
    }


    [HttpPost]
    public async Task<IActionResult> Post(CreateRoomRequest request)
    {
        return Created();
    }
}
