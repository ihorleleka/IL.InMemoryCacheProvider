## Agent Notes

Purpose: In-memory cache provider with tag-based eviction, plus extension methods for get-or-add patterns.

Key paths:
- `Code/CacheProvider/InMemoryCacheProvider.cs` core implementation
- `Code/Extensions/CacheProviderExtensions.cs` get-or-add helpers and locking
- `Tests/Extensions/CacheProviderTests.cs` and `Tests/Extensions/CacheProviderExtensionsTests.cs` unit tests

Concurrency model:
- Mutations are serialized to keep tag indexes consistent.
- Reads are lock-free for throughput and may observe transient states during concurrent mutations.

Build/test entry points:
- Solution file: `IL.InMemoryCacheProvider.slnx`
- Test project: `Tests/IL.InMemoryCacheProvider.Tests.csproj`
