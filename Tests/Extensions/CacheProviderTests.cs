using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IL.InMemoryCacheProvider.Extensions;
using Xunit;

namespace IL.InMemoryCacheProvider.Tests.Extensions
{
    public class CacheProviderTests
    {
        private const string Key = "testkey";
        private const string Tag = "testTag";
        private const string ExpectedValue = "newValue";

        [Fact]
        public async Task GetAllKeysAsync_Returns_ExistingKeys_When_CacheContains_key()
        {
            // Arrange
            var cacheProvider = new CacheProvider.InMemoryCacheProvider();
            await cacheProvider.GetOrAddAsync(Key, () => ExpectedValue);

            // Act
            var result = await cacheProvider.GetAllKeysAsync();

            // Assert
            Assert.Contains(Key, result);
        }

        [Fact]
        public async Task DeleteAllAsync_Deletes_ExistingKeys_When_CacheContains_Objects()
        {
            // Arrange
            var cacheProvider = new CacheProvider.InMemoryCacheProvider();
            await cacheProvider.GetOrAddAsync(Key, () => ExpectedValue);
            await cacheProvider.DeleteAllAsync();

            // Act
            var result = await cacheProvider.GetAllKeysAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task Add_Tags_And_Eviction_By_Tag_For_Cache_Objects()
        {
            // Arrange
            var cacheProvider = new CacheProvider.InMemoryCacheProvider();
            await cacheProvider.GetOrAddAsync(Key,
                () => ExpectedValue,
                tags: [Tag]);
            
            await cacheProvider.EvictByTagAsync(Tag);

            // Act
            var result = await cacheProvider.GetAllKeysAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task Concurrent_Mutations_Complete_Without_Deadlock()
        {
            // Arrange
            var cacheProvider = new CacheProvider.InMemoryCacheProvider();
            var tags = new[] { "t1", "t2", "t3" };
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            // Act
            var tasks = Enumerable.Range(0, 24).Select(workerId => Task.Run(async () =>
            {
                for (var i = 0; i < 200; i++)
                {
                    var key = $"k-{workerId}-{i}";
                    switch (i % 3)
                    {
                        case 0:
                            await cacheProvider.GetOrAddAsync(key, () => ExpectedValue, tags: [tags[i % tags.Length]]);
                            break;
                        case 1:
                            await cacheProvider.EvictByTagAsync(tags[i % tags.Length]);
                            break;
                        default:
                            await cacheProvider.DeleteAllAsync();
                            break;
                    }
                }
            }, cts.Token)).ToArray();

            var all = Task.WhenAll(tasks);
            var completed = await Task.WhenAny(all, Task.Delay(TimeSpan.FromSeconds(5), cts.Token));

            // Assert
            Assert.Same(all, completed);
            await all;
        }
    }
}
