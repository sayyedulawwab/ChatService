using ChatService.Application.Abstractions.Messaging;

namespace ChatService.Application.Rooms.JoinRoom;

public record JoinRoomCommand(long roomId, long userId) : ICommand<long>;
