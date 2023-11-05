using System.Runtime.Caching;

namespace IL.InMemoryCacheProvider.CacheProvider;

public class InMemoryCacheProvider : ICacheProvider
{
    private readonly MemoryCache _cache = MemoryCache.Default;

    public void Add<T>(string key, T? obj, DateTimeOffset? expiration = null, bool useSlidingExpiration = false)
    {
        if (obj == null)
        {
            return;
        }
        
        expiration ??= DateTimeOffset.MaxValue;
        var cacheItemPolicy = useSlidingExpiration switch
        {
            true => new CacheItemPolicy { SlidingExpiration = expiration.Value.Offset },
            false => new CacheItemPolicy { AbsoluteExpiration = expiration.Value }
        };
        _cache.Set(key, obj, cacheItemPolicy);
    }

    public Task AddAsync<T>(string key, T? obj, DateTimeOffset? expiration = null, bool slidingExpiration = false)
    {
        Add(key, obj, expiration, slidingExpiration);
        return Task.CompletedTask;
    }

    public T? Get<T>(string key) => (T)_cache.Get(key);

    public Task<T?> GetAsync<T>(string key)
    {
        return Task.FromResult(Get<T>(key));
    }

    public void Delete(string key)
    {
        _cache.Remove(key);
    }

    public Task DeleteAsync(string key)
    {
        Delete(key);
        return Task.CompletedTask;
    }

    public bool HasKey(string key)
    {
        return _cache.Contains(key);
    }
}