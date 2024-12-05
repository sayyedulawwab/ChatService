namespace ChatService.Application.Rooms;

public class RoomResponse
{
    public string Name { get; init; }
    public List<MemberResponse> Members { get; init; }
}

public class MemberResponse
{
    public string Name { get; init; }
    public string Role { get; init; }
    public DateTime JoinedOn { get; init; }
}