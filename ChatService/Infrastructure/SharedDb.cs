using ChatService.Domain.Entities;
using System.Collections.Concurrent;

namespace ChatService.Infrastructure;

public class SharedDb
{
    private readonly ConcurrentDictionary<string, UserConnection> _connections = new();
    public ConcurrentDictionary<string, UserConnection> connections => _connections;
}
