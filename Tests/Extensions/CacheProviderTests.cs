using IL.InMemoryCacheProvider.Extensions;
using Xunit;

namespace IL.InMemoryCacheProvider.Tests.Extensions
{
    public class CacheProviderTests
    {
        private const string Key = "testkey";
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
    }
}