namespace IL.InMemoryCacheProvider.CacheProvider;

public interface ICacheProvider
{
    void Add<T>(string key, T? obj, DateTimeOffset? expiration = null, bool useSlidingExpiration = false);

    Task AddAsync<T>(string key, T? obj, DateTimeOffset? expiration = null, bool useSlidingExpiration = false);

    T? Get<T>(string key);

    Task<T?> GetAsync<T>(string key);

    void Delete(string key);

    Task DeleteAsync(string key);

    bool HasKey(string key);
}