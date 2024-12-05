using ChatService.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.Json;

namespace ChatService.Infrastructure.Caching;

public class CacheRepository : ICacheRepository
{
    private readonly IDistributedCache _distributedCache;
    private static readonly ConcurrentDictionary<string, bool> CachedKeys = new();

    public CacheRepository(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        string? cachedValue = await _distributedCache.GetStringAsync(key);

        if (cachedValue is null)
        {
            return null;
        }

        T? value = JsonSerializer.Deserialize<T>(cachedValue);

        return value;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var options = new DistributedCacheEntryOptions();

        if (expiration.HasValue)
        {
            options.SetAbsoluteExpiration(expiration.Value);
        }

        string cachedValue = JsonSerializer.Serialize(value);

        await _distributedCache.SetStringAsync(key, cachedValue, options);

        CachedKeys.TryAdd(key, false);

    }

    public async Task<List<T>> GetListAsync<T>(string key) where T : class
    {
        var cachedValue = await _distributedCache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cachedValue))
        {
            return new List<T>();
        }

        return JsonSerializer.Deserialize<List<T>>(cachedValue) ?? new List<T>();
    }

    public async Task AddToListAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        var existingList = await GetListAsync<T>(key);
        existingList.Add(value);

        var serializedList = JsonSerializer.Serialize(existingList);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };

        await _distributedCache.SetStringAsync(key, serializedList, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);

        CachedKeys.TryRemove(key, out bool _);
    }

    public async Task RemoveByPrefixAsync(string prefixKey)
    {
        IEnumerable<Task> tasks = CachedKeys
            .Keys
            .Where(k => k.StartsWith(prefixKey))
            .Select(k => RemoveAsync(k));

        Task.WhenAll(tasks);
    }
}
