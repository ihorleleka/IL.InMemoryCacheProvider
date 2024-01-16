using IL.InMemoryCacheProvider.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace IL.InMemoryCacheProvider.CacheProvider;

public sealed class InMemoryCacheProvider : ICacheProvider
{
    private readonly IMemoryCache _cache;

    public InMemoryCacheProvider(MemoryCacheOptions? options = null)
    {
        _cache = new MemoryCache(options ?? new MemoryCacheOptions());
    }

    public void Add<T>(string key, T? obj, DateTimeOffset? expiration = null, TimeSpan? slidingExpiration = null)
    {
        if (obj == null)
        {
            return;
        }

        var cacheItemPolicy = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = expiration,
            SlidingExpiration = slidingExpiration
        };
        _cache.Set(key, obj, cacheItemPolicy);
    }

    public Task AddAsync<T>(string key, T? obj, DateTimeOffset? expiration = null, TimeSpan? slidingExpiration = null)
    {
        Add(key, obj, expiration, slidingExpiration);
        return Task.CompletedTask;
    }

    public T? Get<T>(string key) => _cache.Get<T>(key);

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
        return _cache.TryGetValue(key, out _);
    }

    public Task<IEnumerable<string>> GetAllKeysAsync()
    {
        return Task.FromResult(GetAllKeys());
    }

    public IEnumerable<string> GetAllKeys()
    {
        return _cache.GetKeys<string>();
    }

    public async Task DeleteAllAsync()
    {
        foreach (var key in await GetAllKeysAsync())
        {
            await DeleteAsync(key);
        }
    }

    public void DeleteAll()
    {
        foreach (var key in GetAllKeys())
        {
            Delete(key);
        }
    }
}