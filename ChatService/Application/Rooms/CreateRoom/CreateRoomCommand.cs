﻿using ChatService.Application.Abstractions.Messaging;

namespace ChatService.Application.Rooms.CreateRoom;

public record CreateRoomCommand(string name, string password, long userId) : ICommand<long>;
