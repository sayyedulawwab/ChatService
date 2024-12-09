using ChatService.Application.Users.Login;
using ChatService.Application.Users.Register;
using ChatService.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers.Users;
[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        var command = new RegisterUserCommand(
            request.name,
            request.email,
            request.password);

        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        var query = new LoginUserQuery(request.email, request.password);

        var result = await _sender.Send(query);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }
}