using IL.InMemoryCacheProvider.CacheProvider;
using IL.Misc.Concurrency;

namespace IL.InMemoryCacheProvider.Extensions;

public static class CacheProviderExtensions
{
    public static T GetOrAdd<T>(this ICacheProvider cacheProvider,
        string key,
        Func<T> valueFactory,
        Func<T, bool>? cacheCreationCondition = null,
        DateTimeOffset? expiration = null,
        TimeSpan? slidingExpiration = null)
    {
        using (LockManager.GetLock(key))
        {
            var value = cacheProvider.GetAsync<T>(key).Result;
            if (value != null)
            {
                return value;
            }

            value = valueFactory();
            AddCacheEntryIfJustified(cacheProvider, key, value, cacheCreationCondition, expiration, slidingExpiration);

            return value;
        }
    }

    public static T GetOrAdd<T>(this ICacheProvider cacheProvider,
        string key,
        Func<Task<T>> valueFactory,
        Func<T, bool>? cacheCreationCondition = null,
        DateTimeOffset? expiration = null,
        TimeSpan? slidingExpiration = null)
    {
        using (LockManager.GetLock(key))
        {
            var value = cacheProvider.GetAsync<T>(key).Result;
            if (value != null)
            {
                return value;
            }

            value = valueFactory().Result;
            AddCacheEntryIfJustified(cacheProvider, key, value, cacheCreationCondition, expiration, slidingExpiration);

            return value;
        }
    }

    private static void AddCacheEntryIfJustified<T>(ICacheProvider cacheProvider, string key,
        T value, Func<T, bool>? cacheCreationCondition, DateTimeOffset? expiration, TimeSpan? slidingExpiration = null)
    {
        if (cacheCreationCondition is null || cacheCreationCondition(value))
        {
            cacheProvider.AddAsync(key, value, expiration, slidingExpiration).Wait();
        }
    }

    public static async Task<T> GetOrAddAsync<T>(this ICacheProvider cacheProvider,
        string key,
        Func<T> valueFactory,
        Func<T, bool>? cacheCreationCondition = null,
        DateTimeOffset? expiration = null,
        TimeSpan? slidingExpiration = null)
    {
        using (await LockManager.GetLockAsync(key))
        {
            var value = await cacheProvider.GetAsync<T>(key);
            if (value != null)
            {
                return value;
            }

            value = valueFactory();
            await AddCacheEntryIfJustifiedAsync(cacheProvider, key, value, cacheCreationCondition, expiration, slidingExpiration);

            return value;
        }
    }

    public static async Task<T> GetOrAddAsync<T>(this ICacheProvider cacheProvider,
        string key,
        Func<Task<T>> valueFactory,
        Func<T, bool>? cacheCreationCondition = null,
        DateTimeOffset? expiration = null,
        TimeSpan? slidingExpiration = null)
    {
        using (await LockManager.GetLockAsync(key))
        {
            var value = await cacheProvider.GetAsync<T>(key);
            if (value != null)
            {
                return value;
            }

            value = await valueFactory();
            await AddCacheEntryIfJustifiedAsync(cacheProvider, key, value, cacheCreationCondition, expiration, slidingExpiration);

            return value;
        }
    }

    private static async Task AddCacheEntryIfJustifiedAsync<T>(ICacheProvider cacheProvider, string key,
        T value, Func<T, bool>? cacheCreationCondition, DateTimeOffset? expiration, TimeSpan? slidingExpiration = null)
    {
        if (cacheCreationCondition is null || cacheCreationCondition(value))
        {
            await cacheProvider.AddAsync(key, value, expiration, slidingExpiration);
        }
    }
}