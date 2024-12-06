using ChatService.Application.Abstractions.Caching;
using ChatService.Domain.Users;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace ChatService.Hubs;

public class ChatHub : Hub
{
    private readonly ICacheRepository _cacheRepository;

    public ChatHub(ICacheRepository cacheRepository)
    {
        _cacheRepository = cacheRepository;
    }


    public async Task JoinSpecificChatRoom(UserConnection connection)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

        await _cacheRepository.SetAsync($"connection-{Context.ConnectionId}", connection, null);
        await _cacheRepository.SetAsync($"username-{connection.Username}", Context.ConnectionId, null);

        await Clients.Group(connection.ChatRoom).SendAsync("JoinSpecificChatRoom", "admin", $"{connection.Username} has joined");
    }

    public async Task SendMessage(string message)
    {
        var cachedConnection = await _cacheRepository.GetAsync<UserConnection>($"connection-{Context.ConnectionId}");

        if (cachedConnection is not null)
        {
            var chatMessage = new
            {
                Username = cachedConnection.Username,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            // Add message to chat room history
            await _cacheRepository.AddToListAsync($"chatroom-{cachedConnection.ChatRoom}", chatMessage);


            await Clients.Group(cachedConnection.ChatRoom).SendAsync("ReceiveSpecificMessage", cachedConnection.Username, message);
        }
    }

    //public async Task<List<object>> GetChatHistory(string chatRoom)
    //{
    //    // Fetch chat history for the room
    //    return await _cacheRepository.GetListAsync<object>($"chatroom-{chatRoom}");
    //}

    public async Task Typing(string chatRoom, string username)
    {
        // Store typing state with an expiration
        //await _cacheRepository.SetAsync($"typing-{chatRoom}-{username}", true, TimeSpan.FromSeconds(3));

        await Clients.Group(chatRoom).SendAsync("UserTyping", username);
    }

    public async Task Reconnect(string connectionId)
    {
        var state = await _cacheRepository.GetAsync<UserConnection>($"connection-{connectionId}");
        if (state is not null)
        {
            var username = state.Username;
            var chatRoom = state.ChatRoom;

            // Add the connection back to the group
            await Groups.AddToGroupAsync(connectionId, chatRoom);

            // Notify others in the group
            await Clients.Group(chatRoom).SendAsync("UserReconnected", username, $"{username} has reconnected");
        }
    }

}
