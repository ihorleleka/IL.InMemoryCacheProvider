using IL.InMemoryCacheProvider.Options;

namespace IL.InMemoryCacheProvider.CacheProvider;

public interface ICacheProvider
{
    void Add<T>(string key, T? obj, ExpirationOptions? expirationOptions = default, IEnumerable<string>? tags = default);

    Task AddAsync<T>(string key, T? obj, ExpirationOptions? expirationOptions = default, IEnumerable<string>? tags = default);

    T? Get<T>(string key);

    Task<T?> GetAsync<T>(string key);

    void Delete(string key);

    Task DeleteAsync(string key);
    
    void EvictByTag(string tag);

    Task EvictByTagAsync(string tag);

    bool HasKey(string key);

    Task<IEnumerable<string>> GetAllKeysAsync(Predicate<string>? filter = default);

    IEnumerable<string> GetAllKeys(Predicate<string>? filter = default);

    Task DeleteAllAsync(Predicate<string>? filter = default);

    void DeleteAll(Predicate<string>? filter = default);
}