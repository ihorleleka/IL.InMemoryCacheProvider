using System.Collections.Concurrent;
using IL.InMemoryCacheProvider.Options;
using Microsoft.Extensions.Caching.Memory;

namespace IL.InMemoryCacheProvider.CacheProvider;

public sealed class InMemoryCacheProvider(MemoryCacheOptions? options = null) : ICacheProvider
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _tagIndex = new();
#if NET8_0
    private readonly ConcurrentDictionary<string, byte> _allKeys = new();
#endif
    private readonly MemoryCache _cache = new(options ?? new MemoryCacheOptions());

    public void Add<T>(string key, T? obj, ExpirationOptions? expirationOptions = null, params string[] tags)
    {
        if (obj == null)
        {
            return;
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationOptions?.AbsoluteExpirationRelativeToNow,
            AbsoluteExpiration = expirationOptions?.AbsoluteExpiration,
            SlidingExpiration = expirationOptions?.SlidingExpiration
        };

#if NET8_0
        _allKeys.TryAdd(key, 0);
#endif
        foreach (var tag in tags)
        {
            _tagIndex.AddOrUpdate(tag,
                _ =>
                {
                    var keys = new ConcurrentDictionary<string, byte>();
                    keys.TryAdd(key, 0);
                    return keys;
                },
                (_, keys) =>
                {
                    keys.TryAdd(key, 0);
                    return keys;
                });
        }

        cacheEntryOptions.RegisterPostEvictionCallback((echoKey, _, reason, _) =>
        {
            var k = (string)echoKey;
#if NET8_0
            if (reason != EvictionReason.Replaced)
            {
                _allKeys.TryRemove(k, out _);
            }
#endif

            if (tags.Length == 0)
            {
                return;
            }

            foreach (var tag in tags)
            {
                if (!_tagIndex.TryGetValue(tag, out var tagKeys))
                {
                    continue;
                }

                tagKeys.TryRemove(k, out _);
                if (tagKeys.IsEmpty)
                {
                    _tagIndex.TryRemove(tag, out _);
                }
            }
        });

        _cache.Set(key, obj, cacheEntryOptions);
    }

    public Task AddAsync<T>(string key,
        T? obj,
        ExpirationOptions? expirationOptions = null,
        params string[] tags)
    {
        Add(key, obj, expirationOptions, tags);
        return Task.CompletedTask;
    }

    public T? Get<T>(string key) => _cache.Get<T>(key);

    public Task<T?> GetAsync<T>(string key)
    {
        return Task.FromResult(Get<T>(key));
    }

    public void Delete(string key)
    {
#if NET8_0
        _allKeys.TryRemove(key, out _);
#endif
        _cache.Remove(key);
    }

    public Task DeleteAsync(string key)
    {
        Delete(key);
        return Task.CompletedTask;
    }

    public void EvictByTag(string tag)
    {
        if (!_tagIndex.TryRemove(tag, out var tagKeys))
        {
            return;
        }

        foreach (var cacheKey in tagKeys.Keys)
        {
            Delete(cacheKey);
        }
    }

    public Task EvictByTagAsync(string tag)
    {
        EvictByTag(tag);
        return Task.CompletedTask;
    }

    public bool HasKey(string key)
    {
        return _cache.TryGetValue(key, out _);
    }

    public Task<IEnumerable<string>> GetAllKeysAsync(Predicate<string>? filter = null)
    {
        return Task.FromResult(GetAllKeys(filter));
    }

    public IEnumerable<string> GetAllKeys(Predicate<string>? filter = null)
    {
#if NET8_0
        return _allKeys.Keys.Where(cacheKey => filter == null || filter(cacheKey));
#else
        return _cache.Keys
            .OfType<string>()
            .Where(cacheKey => filter == null || filter(cacheKey));
#endif
    }

    public Task DeleteAllAsync(Predicate<string>? filter = null)
    {
        DeleteAll(filter);
        return Task.CompletedTask;
    }

    public void DeleteAll(Predicate<string>? filter = null)
    {
        foreach (var key in GetAllKeys(filter))
        {
            Delete(key);
        }

        if (filter != null)
        {
            return;
        }

        _tagIndex.Clear();
#if NET8_0
        _allKeys.Clear();
#endif
        // The specialized cleanup for tags is harder with filter, 
        // but Delete(key) triggers EvictionCallback which handles tag cleanup.
        // So we just need to ensure Delete(key) is called.
        // The explicit loop above handles it.
        // TagIndex cleanup is automatic via callbacks.
    }
}