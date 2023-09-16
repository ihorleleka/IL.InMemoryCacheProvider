using IL.InMemoryCacheProvider.CacheProvider;
using IL.InMemoryCacheProvider.Extensions;
using Moq;
using Xunit;

namespace IL.InMemoryCacheProvider.Tests.Extensions
{
    public class CacheProviderExtensionsTests
    {
        [Fact]
        public async Task GetOrAddAsync_Returns_ExistingValue_When_CacheContainsKey()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            var key = "testKey";
            var expectedValue = "newValue";

            cacheServiceMock.Setup(x => x.GetAsync<string>(key)).ReturnsAsync(expectedValue);

            // Act
            var result = await cacheServiceMock.Object.GetOrAddAsync(
                key,
                () =>
                {
                    throw new InvalidOperationException("Value factory should not be called.");
                    return string.Empty;
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public async Task GetOrAddAsync_Returns_NewValue_When_CacheDoesNotContainKey()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            var key = "testKey";
            var expectedValue = "newValue";
            var valueFactoryCalled = false;

            cacheServiceMock.Setup(x => x.GetAsync<string>(key))
                .ReturnsAsync((string)null);

            // Act
            var result = await cacheServiceMock.Object.GetOrAddAsync(key,
                () =>
                {
                    valueFactoryCalled = true;
                    return expectedValue;
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(expectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheServiceMock.Verify(x => x.AddAsync(key, expectedValue, null), Times.Once);
        }

        [Fact]
        public async Task GetOrAddAsync_AsyncFactory_Returns_ExistingValue_When_CacheContainsKey()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            var key = "testKey";
            var expectedValue = "newValue";

            cacheServiceMock.Setup(x => x.GetAsync<string>(key)).ReturnsAsync(expectedValue);

            // Act
            var result = await cacheServiceMock.Object.GetOrAddAsync(
                key,
                async () =>
                    {
                        throw new InvalidOperationException("Value factory should not be called.");
                        return string.Empty;
                    },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public async Task GetOrAddAsync_AsyncFactory_Returns_NewValue_When_CacheDoesNotContainKey()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            var key = "testKey";
            var expectedValue = "newValue";
            var valueFactoryCalled = false;

            cacheServiceMock.Setup(x => x.GetAsync<string>(key))
                .ReturnsAsync((string)null);

            // Act
            var result = await cacheServiceMock.Object.GetOrAddAsync(key,
                async () =>
                {
                    valueFactoryCalled = true;
                    return expectedValue;
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(expectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheServiceMock.Verify(x => x.AddAsync(key, expectedValue, null), Times.Once);
        }

        [Fact]
        public void GetOrAdd_Returns_ExistingValue_When_CacheContainsKey()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            var key = "testKey";
            var expectedValue = "newValue";

            cacheServiceMock.Setup(x => x.GetAsync<string>(key)).ReturnsAsync(expectedValue);

            // Act
            var result = cacheServiceMock.Object.GetOrAdd(
                key,
                () =>
                {
                    throw new InvalidOperationException("Value factory should not be called.");
                    return string.Empty;
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetOrAdd_Returns_NewValue_When_CacheDoesNotContainKey()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            var key = "testKey";
            var expectedValue = "newValue";
            var valueFactoryCalled = false;

            cacheServiceMock.Setup(x => x.GetAsync<string>(key))
                .ReturnsAsync((string)null);

            // Act
            var result = cacheServiceMock.Object.GetOrAdd(key,
                () =>
                {
                    valueFactoryCalled = true;
                    return expectedValue;
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(expectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheServiceMock.Verify(x => x.AddAsync(key, expectedValue, null), Times.Once);
        }

        [Fact]
        public void GetOrAdd_AsyncFactory_Returns_ExistingValue_When_CacheContainsKey()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            var key = "testKey";
            var expectedValue = "newValue";

            cacheServiceMock.Setup(x => x.GetAsync<string>(key)).ReturnsAsync(expectedValue);

            // Act
            var result = cacheServiceMock.Object.GetOrAdd(
                key,
                async () =>
                {
                    throw new InvalidOperationException("Value factory should not be called.");
                    return string.Empty;
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetOrAdd_AsyncFactory_Returns_NewValue_When_CacheDoesNotContainKey()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            var key = "testKey";
            var expectedValue = "newValue";
            var valueFactoryCalled = false;

            cacheServiceMock.Setup(x => x.GetAsync<string>(key))
                .ReturnsAsync((string)null);

            // Act
            var result = cacheServiceMock.Object.GetOrAdd(key,
                async () =>
                {
                    valueFactoryCalled = true;
                    return expectedValue;
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(expectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheServiceMock.Verify(x => x.AddAsync(key, expectedValue, null), Times.Once);
        }
    }
}