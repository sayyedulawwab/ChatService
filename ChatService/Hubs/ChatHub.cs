using ChatService.Application.Abstractions.Caching;
using ChatService.Application.Conversations.CreateConversation;
using ChatService.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatService.Hubs;

public class ChatHub : Hub
{
    private readonly ICacheRepository _cacheRepository;
    private readonly ISender _sender;
    private readonly JwtOptions _jwtOptions;

    public ChatHub(ICacheRepository cacheRepository, ISender sender, IOptions<JwtOptions> jwtOptions)
    {
        _cacheRepository = cacheRepository;
        _sender = sender;
        _jwtOptions = jwtOptions.Value;
    }

    public override async Task OnConnectedAsync()
    {
        var token = Context.GetHttpContext()?.Request.Query["access_token"].ToString();

        if (string.IsNullOrEmpty(token))
        {
            await Clients.Caller.SendAsync("Error", "Authentication failed: No token provided");
            Context.Abort();
            return;
        }

        if (!ValidateJwtToken(token))
        {
            await Clients.Caller.SendAsync("Error", "Authentication failed: Invalid token");
            Context.Abort();
            return;
        }

        await base.OnConnectedAsync();
    }

    private bool ValidateJwtToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

            return principal.Identity.IsAuthenticated;
            
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Retrieve the cached connection information
        var cachedConnection = await _cacheRepository.GetAsync<UserConnection>($"connection-{Context.ConnectionId}");
        if (cachedConnection is not null)
        {
            // Clean up the cache and notify the group
            await _cacheRepository.RemoveAsync($"connection-{Context.ConnectionId}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, cachedConnection.roomId);
            await Clients.Group(cachedConnection.roomId)
                         .SendAsync("UserLeft", cachedConnection.username);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinSpecificChatRoom(UserConnection connection)
    {
        // Add the connection to the group and cache the user details
        await Groups.AddToGroupAsync(Context.ConnectionId, connection.roomId);
        await _cacheRepository.SetAsync($"connection-{Context.ConnectionId}", connection, TimeSpan.FromHours(1));

        // Notify group members about the new user
        await Clients.Group(connection.roomId)
                     .SendAsync("UserJoined", "admin", $"{connection.username} has joined");
    }

    public async Task SendMessage(string roomId, string senderId, string content)
    {
        // Fetch the cached connection details
        var cachedConnection = await _cacheRepository.GetAsync<UserConnection>($"connection-{Context.ConnectionId}");
        if (cachedConnection is null)
        {
            await Clients.Caller.SendAsync("Error", "User not in any chat room.");
            return;
        }

        // Prepare the message and cache it in the room's chat history
        var message = new
        {
            Username = cachedConnection.username,
            Message = content,
            Timestamp = DateTime.UtcNow
        };
        await _cacheRepository.AddToListAsync($"room-{cachedConnection.roomId}", message);

        // Persist the message to the database
        var command = new AddMessageCommand(Convert.ToInt64(roomId), Convert.ToInt64(senderId), content);
        await _sender.Send(command);

        // Broadcast the message to the group
        await Clients.Group(cachedConnection.roomId)
                     .SendAsync("ReceiveMessage", cachedConnection.username, content);
    }

    public async Task Typing(string roomId, string username)
    {
        // Store typing state with an expiration
        await Clients.Group(roomId).SendAsync("UserTyping", username);
    }

}
