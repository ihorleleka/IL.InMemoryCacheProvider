using IL.InMemoryCacheProvider.CacheProvider;
using IL.InMemoryCacheProvider.Options;
using IL.Misc.Concurrency;

namespace IL.InMemoryCacheProvider.Extensions;

public static class CacheProviderExtensions
{
    public static T GetOrAdd<T>(this ICacheProvider cacheProvider,
        string key,
        Func<T> valueFactory,
        Func<T, bool>? cacheCreationCondition = default,
        ExpirationOptions? expirationOptions = default,
        IEnumerable<string>? tags = default)
    {
        using (LockManager.GetLock(key))
        {
            var value = cacheProvider.GetAsync<T>(key).Result;
            if (value != null)
            {
                return value;
            }

            value = valueFactory();
            AddCacheEntryIfJustified(cacheProvider, key, value, cacheCreationCondition, expirationOptions, tags);

            return value;
        }
    }

    public static T GetOrAdd<T>(this ICacheProvider cacheProvider,
        string key,
        Func<Task<T>> valueFactory,
        Func<T, bool>? cacheCreationCondition = default,
        ExpirationOptions? expirationOptions = default,
        IEnumerable<string>? tags = default)
    {
        using (LockManager.GetLock(key))
        {
            var value = cacheProvider.GetAsync<T>(key).Result;
            if (value != null)
            {
                return value;
            }

            value = valueFactory().Result;
            AddCacheEntryIfJustified(cacheProvider, key, value, cacheCreationCondition, expirationOptions, tags);

            return value;
        }
    }

    private static void AddCacheEntryIfJustified<T>(ICacheProvider cacheProvider,
        string key,
        T value,
        Func<T, bool>? cacheCreationCondition,
        ExpirationOptions? expirationOptions, 
        IEnumerable<string>? tags)
    {
        if (cacheCreationCondition is null || cacheCreationCondition(value))
        {
            cacheProvider.AddAsync(key, value, expirationOptions, tags).Wait();
        }
    }

    public static async Task<T> GetOrAddAsync<T>(this ICacheProvider cacheProvider,
        string key,
        Func<T> valueFactory,
        Func<T, bool>? cacheCreationCondition = default,
        ExpirationOptions? expirationOptions = default,
        IEnumerable<string>? tags = default)
    {
        using (await LockManager.GetLockAsync(key))
        {
            var value = await cacheProvider.GetAsync<T>(key);
            if (value != null)
            {
                return value;
            }

            value = valueFactory();
            await AddCacheEntryIfJustifiedAsync(cacheProvider, key, value, cacheCreationCondition, expirationOptions, tags);

            return value;
        }
    }

    public static async Task<T> GetOrAddAsync<T>(this ICacheProvider cacheProvider,
        string key,
        Func<Task<T>> valueFactory,
        Func<T, bool>? cacheCreationCondition = default,
        ExpirationOptions? expirationOptions = default,
        IEnumerable<string>? tags = default)
    {
        using (await LockManager.GetLockAsync(key))
        {
            var value = await cacheProvider.GetAsync<T>(key);
            if (value != null)
            {
                return value;
            }

            value = await valueFactory();
            await AddCacheEntryIfJustifiedAsync(cacheProvider, key, value, cacheCreationCondition, expirationOptions, tags);

            return value;
        }
    }

    private static async Task AddCacheEntryIfJustifiedAsync<T>(ICacheProvider cacheProvider,
        string key,
        T value,
        Func<T, bool>? cacheCreationCondition,
        ExpirationOptions? expirationOptions, 
        IEnumerable<string>? tags)
    {
        if (cacheCreationCondition is null || cacheCreationCondition(value))
        {
            await cacheProvider.AddAsync(key, value, expirationOptions, tags);
        }
    }
}