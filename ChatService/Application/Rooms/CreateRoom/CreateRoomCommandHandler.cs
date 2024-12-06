using ChatService.Application.Abstractions.Auth;
using ChatService.Application.Abstractions.Clock;
using ChatService.Application.Abstractions.Messaging;
using ChatService.Domain.Abstractions;
using ChatService.Domain.Rooms;

namespace ChatService.Application.Rooms.CreateRoom;

internal sealed class CreateRoomCommandHandler : ICommandHandler<CreateRoomCommand, long>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPasswordHasher _passwordHasher;

    public CreateRoomCommandHandler(IRoomRepository roomRepository, IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _roomRepository = roomRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<long>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var passwordSalt = _passwordHasher.GenerateSalt();
        var hashedPassword = _passwordHasher.Hash(request.password, passwordSalt);

        var room = Room.Create(request.name, hashedPassword, _dateTimeProvider.UtcNow);

        room.AddMember(request.userId, room.Id, Role.Admin, _dateTimeProvider.UtcNow);

        _roomRepository.Add(room);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return room.Id;
    }
}