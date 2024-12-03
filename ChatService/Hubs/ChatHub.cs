using ChatService.Domain.Entities;
using ChatService.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs;

public class ChatHub : Hub
{
    private readonly SharedDb _shared;

    public ChatHub(SharedDb shared)
    {
        _shared = shared;
    }

    public async Task JoinChat(UserConnection connection)
    {
        await Clients.All.SendAsync("ReceiveMessage", "admin", $"{connection.Username} has joined");
    }

    public async Task JoinSpecificChatRoom(UserConnection connection)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

        _shared.connections[Context.ConnectionId] = connection;

        await Clients.Group(connection.ChatRoom).SendAsync("JoinSpecificChatRoom", "admin", $"{connection.Username} has joined");
    }

    public async Task SendMessage(string message)
    {
        if (_shared.connections.TryGetValue(Context.ConnectionId, out UserConnection connection))
        {
            await Clients.Group(connection.ChatRoom).SendAsync("ReceiveSpecificMessage", connection.Username, message);
        }
    }

    public async Task Typing(string chatRoom, string username)
    {
        // Notify others in the same chat room that this user is typing
        await Clients.Group(chatRoom).SendAsync("UserTyping", username);
    }
}
