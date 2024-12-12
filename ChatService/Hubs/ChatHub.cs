using ChatService.Application.Abstractions.Caching;
using ChatService.Application.Conversations.CreateConversation;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs;

public class ChatHub : Hub
{
    private readonly ICacheRepository _cacheRepository;
    private readonly ISender _sender;

    public ChatHub(ICacheRepository cacheRepository, ISender sender)
    {
        _cacheRepository = cacheRepository;
        _sender = sender;
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
