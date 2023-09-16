using IL.InMemoryCacheProvider.CacheProvider;
using IL.Misc.Concurrency;

namespace IL.InMemoryCacheProvider.Extensions;
public static class CacheProviderExtensions
{
    public static T GetOrAdd<T>(this ICacheProvider cacheService,
        string key,
        Func<T> valueFactory,
        Func<T, bool>? cacheCreationCondition = null,
        DateTimeOffset? absoluteExpiration = null)
    {
        using (LockManager.GetLock(key))
        {
            var value = cacheService.GetAsync<T>(key).Result;
            if (value != null)
            {
                return value;
            }

            value = valueFactory();
            AddCacheEntryIfJustified(cacheService, key, value, cacheCreationCondition, absoluteExpiration);

            return value;
        }
    }

    public static T GetOrAdd<T>(this ICacheProvider rankedCacheService,
        string key,
        Func<Task<T>> valueFactory,
        Func<T, bool>? cacheCreationCondition = null,
        DateTimeOffset? absoluteExpiration = null)
    {
        using (LockManager.GetLock(key))
        {
            var value = rankedCacheService.GetAsync<T>(key).Result;
            if (value != null)
            {
                return value;
            }

            value = valueFactory().Result;
            AddCacheEntryIfJustified(rankedCacheService, key, value, cacheCreationCondition, absoluteExpiration);

            return value;
        }
    }

    private static void AddCacheEntryIfJustified<T>(ICacheProvider rankedCacheService, string key,
        T value, Func<T, bool>? cacheCreationCondition, DateTimeOffset? absoluteExpiration)
    {
        if (cacheCreationCondition is null || cacheCreationCondition(value))
        {
            rankedCacheService.AddAsync(key, value, absoluteExpiration).Wait();
        }
    }

    public static async Task<T> GetOrAddAsync<T>(this ICacheProvider rankedCacheService,
        string key,
        Func<T> valueFactory,
        Func<T, bool>? cacheCreationCondition = null,
        DateTimeOffset? absoluteExpiration = null)
    {
        using (await LockManager.GetLockAsync(key))
        {
            var value = await rankedCacheService.GetAsync<T>(key);
            if (value != null)
            {
                return value;
            }

            value = valueFactory();
            await AddCacheEntryIfJustifiedAsync(rankedCacheService, key, value, cacheCreationCondition, absoluteExpiration);

            return value;
        }
    }

    public static async Task<T> GetOrAddAsync<T>(this ICacheProvider rankedCacheService,
        string key,
        Func<Task<T>> valueFactory,
        Func<T, bool>? cacheCreationCondition = null,
        DateTimeOffset? absoluteExpiration = null)
    {
        using (await LockManager.GetLockAsync(key))
        {
            var value = await rankedCacheService.GetAsync<T>(key);
            if (value != null)
            {
                return value;
            }

            value = await valueFactory();
            await AddCacheEntryIfJustifiedAsync(rankedCacheService, key, value, cacheCreationCondition, absoluteExpiration);

            return value;
        }
    }

    private static async Task AddCacheEntryIfJustifiedAsync<T>(ICacheProvider rankedCacheService, string key,
        T value, Func<T, bool>? cacheCreationCondition, DateTimeOffset? absoluteExpiration)
    {
        if (cacheCreationCondition is null || cacheCreationCondition(value))
        {
            await rankedCacheService.AddAsync(key, value, absoluteExpiration);
        }
    }
}
