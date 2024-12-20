﻿using ChatService.Application.Abstractions.Messaging;

namespace ChatService.Application.Conversations.CreateConversation;

public record CreateConversationCommand(long? roomId, List<long> participants) : ICommand<string>;