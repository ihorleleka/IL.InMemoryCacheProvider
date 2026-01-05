using IL.InMemoryCacheProvider.CacheProvider;
using IL.InMemoryCacheProvider.Options;
using IL.Misc.Concurrency;

namespace IL.InMemoryCacheProvider.Extensions;

public static class CacheProviderExtensions
{
    public static T GetOrAdd<T>(this ICacheProvider cacheProvider,
        string key,
        Func<T> valueFactory,
        Predicate<T>? cacheCreationCondition = null,
        ExpirationOptions? expirationOptions = null,
        params string[] tags)
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
        Predicate<T>? cacheCreationCondition = null,
        ExpirationOptions? expirationOptions = null,
        params string[] tags)
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
        Predicate<T>? cacheCreationCondition,
        ExpirationOptions? expirationOptions,
        params string[] tags)
    {
        if (cacheCreationCondition is null || cacheCreationCondition(value))
        {
            cacheProvider.AddAsync(key, value, expirationOptions, tags).Wait();
        }
    }

    public static async Task<T> GetOrAddAsync<T>(this ICacheProvider cacheProvider,
        string key,
        Func<T> valueFactory,
        Predicate<T>? cacheCreationCondition = null,
        ExpirationOptions? expirationOptions = null,
        CancellationToken cancellationToken = default,
        params string[] tags)
    {
        using (await LockManager.GetLockAsync(key, cancellationToken: cancellationToken))
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
        Predicate<T>? cacheCreationCondition = null,
        ExpirationOptions? expirationOptions = null,
        CancellationToken cancellationToken = default,
        params string[] tags)
    {
        using (await LockManager.GetLockAsync(key, cancellationToken: cancellationToken))
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
        Predicate<T>? cacheCreationCondition,
        ExpirationOptions? expirationOptions,
        params string[] tags)
    {
        if (cacheCreationCondition is null || cacheCreationCondition(value))
        {
            await cacheProvider.AddAsync(key, value, expirationOptions, tags);
        }
    }
}