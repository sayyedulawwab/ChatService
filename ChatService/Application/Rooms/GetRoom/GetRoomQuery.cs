using ChatService.Application.Abstractions.Messaging;

namespace ChatService.Application.Rooms.GetRoom;

public record GetRoomQuery(long roomId) : IQuery<RoomResponse>;
