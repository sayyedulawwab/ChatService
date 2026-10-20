using ChatService.Application.Abstractions.Messaging;
using ChatService.Application.Users.Login;
using ChatService.Application.Users.Register;
using ChatService.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers.Users;
[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request, ICommandHandler<RegisterUserCommand, long> handler, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.name,
            request.email,
            request.password);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserRequest request, IQueryHandler<LoginUserQuery, AccessTokenResponse> handler, CancellationToken cancellationToken)
    {
        var query = new LoginUserQuery(request.email, request.password);

        var result = await handler.Handle(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }
}
