[![NuGet version (IL.InMemoryCacheProvider)](https://img.shields.io/nuget/v/IL.InMemoryCacheProvider.svg?style=flat-square)](https://www.nuget.org/packages/IL.InMemoryCacheProvider/)
# InMemory Cache Provider

* Basic implementation of generic in-memory cache provider, based on Microsoft.Extensions.Caching.Memory.
* Comes with set of useful thread safe extensions to get objects from cache or add them with factory.
* Introduces several not supported by default methods, like `IEnumerable<string> GetAllKeys()` or `void DeleteAll()`.
* Supports tags on cache entries and eviction by tag.

## Concurrency Notes

* Mutating operations (`Add`, `Delete`, `EvictByTag`, `DeleteAll` and async variants) are serialized to keep tag indexes consistent under concurrent use.
* Read operations (`Get`, `HasKey`, `GetAllKeys` and async variants) are lock-free for performance. This means reads may observe transient states during concurrent mutations, which is expected for a non-transactional cache.

## Usage sample:

Register in DI with any container of your preference. Microsoft.Extensions.DependencyInjection will be used in this sample.

`services.AddSingleton<ICacheProvider, InMemoryCacheProvider>();`

```
class Test(ICacheProvider cacheProvider)
{

  void Foo(CancellationToken cancellationToken = default)
  {
    var cacheKey = "test";
    var cacheTags = ["test1", "test2"];
    var valueFromCache = cacheProvider.GetOrAdd(cacheKey,
                CacheValueFactory,
                tags: cacheTags,
                cacheCreationCondition: s => !string.IsNullOrEmpty(s),
                expirationOptions: new ExpirationOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromSeconds(30)
                },
                cancellationToken: cancellationToken);
  }

  private string CacheValueFactory() => return "test value";

}

```

## Nuget
  https://www.nuget.org/packages/IL.InMemoryCacheProvider/
