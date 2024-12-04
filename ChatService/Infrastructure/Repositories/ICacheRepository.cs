namespace ChatService.Infrastructure.Repositories;

public interface ICacheRepository
{
    Task<T?> GetAsync<T>(string key)
        where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiration)
        where T : class;
    Task<List<T>> GetListAsync<T>(string key) where T : class;
    Task AddToListAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
    Task RemoveAsync(string key);
    Task RemoveByPrefixAsync(string prefixKey);
}
