using IL.InMemoryCacheProvider.Options;

namespace IL.InMemoryCacheProvider.CacheProvider;

public interface ICacheProvider
{
    void Add<T>(string key, T? obj, ExpirationOptions? expirationOptions = null, params string[] tags);

    Task AddAsync<T>(string key, T? obj, ExpirationOptions? expirationOptions = null, params string[] tags);

    T? Get<T>(string key);

    Task<T?> GetAsync<T>(string key);

    void Delete(string key);

    Task DeleteAsync(string key);
    
    void EvictByTag(string tag);

    Task EvictByTagAsync(string tag);

    bool HasKey(string key);

    Task<IEnumerable<string>> GetAllKeysAsync(Predicate<string>? filter = null);

    IEnumerable<string> GetAllKeys(Predicate<string>? filter = null);

    Task DeleteAllAsync(Predicate<string>? filter = null);

    void DeleteAll(Predicate<string>? filter = null);
}