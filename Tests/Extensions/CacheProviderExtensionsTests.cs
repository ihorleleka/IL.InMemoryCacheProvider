using IL.InMemoryCacheProvider.CacheProvider;
using IL.InMemoryCacheProvider.Extensions;
using Moq;
using Xunit;

namespace IL.InMemoryCacheProvider.Tests.Extensions
{
    public class CacheProviderExtensionsTests
    {
        private const string Key = "testkey";
        private const string ExpectedValue = "newValue";
        
        [Fact]
        public async Task GetOrAddAsync_Returns_ExistingValue_When_CacheContains_key()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();

            cacheServiceMock.Setup(x => x.GetAsync<string>(Key)).ReturnsAsync(ExpectedValue);

            // Act
            var result = await cacheServiceMock.Object.GetOrAddAsync(
                Key,
                () =>
                {
                    throw new InvalidOperationException("Value factory should not be called.");
#pragma warning disable CS0162 // Unreachable code detected
                    return string.Empty;
#pragma warning restore CS0162 // Unreachable code detected
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(ExpectedValue, result);
        }

        [Fact]
        public async Task GetOrAddAsync_Returns_NewValue_When_CacheDoesNotContain_key()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            var valueFactoryCalled = false;

            cacheServiceMock.Setup(x => x.GetAsync<string>(Key))
                .ReturnsAsync((string)null!);

            // Act
            var result = await cacheServiceMock.Object.GetOrAddAsync(Key,
                () =>
                {
                    valueFactoryCalled = true;
                    return ExpectedValue;
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(ExpectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheServiceMock.Verify(x => x.AddAsync(Key, ExpectedValue, null, false), Times.Once);
        }

        [Fact]
        public async Task GetOrAddAsync_AsyncFactory_Returns_ExistingValue_When_CacheContains_key()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();

            cacheServiceMock.Setup(x => x.GetAsync<string>(Key)).ReturnsAsync(ExpectedValue);

            // Act
            var result = await cacheServiceMock.Object.GetOrAddAsync(
                Key, () =>
                    {
                        throw new InvalidOperationException("Value factory should not be called.");
#pragma warning disable CS0162 // Unreachable code detected
                        return Task.FromResult(string.Empty);
#pragma warning restore CS0162 // Unreachable code detected
                    },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(ExpectedValue, result);
        }

        [Fact]
        public async Task GetOrAddAsync_AsyncFactory_Returns_NewValue_When_CacheDoesNotContain_key()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            var valueFactoryCalled = false;

            cacheServiceMock.Setup(x => x.GetAsync<string>(Key))
                .ReturnsAsync((string)null!);

            // Act
            var result = await cacheServiceMock.Object.GetOrAddAsync(Key, () =>
                {
                    valueFactoryCalled = true;
                    return Task.FromResult(ExpectedValue);
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(ExpectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheServiceMock.Verify(x => x.AddAsync(Key, ExpectedValue, null, false), Times.Once);
        }

        [Fact]
        public void GetOrAdd_Returns_ExistingValue_When_CacheContains_key()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            cacheServiceMock.Setup(x => x.GetAsync<string>(Key)).ReturnsAsync(ExpectedValue);

            // Act
            var result = cacheServiceMock.Object.GetOrAdd(
                Key,
                () =>
                {
                    throw new InvalidOperationException("Value factory should not be called.");
#pragma warning disable CS0162 // Unreachable code detected
                    return string.Empty;
#pragma warning restore CS0162 // Unreachable code detected
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(ExpectedValue, result);
        }

        [Fact]
        public void GetOrAdd_Returns_NewValue_When_CacheDoesNotContain_key()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            var valueFactoryCalled = false;

            cacheServiceMock.Setup(x => x.GetAsync<string>(Key))
                .ReturnsAsync((string)null!);

            // Act
            var result = cacheServiceMock.Object.GetOrAdd(Key,
                () =>
                {
                    valueFactoryCalled = true;
                    return ExpectedValue;
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(ExpectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheServiceMock.Verify(x => x.AddAsync(Key, ExpectedValue, null, false), Times.Once);
        }

        [Fact]
        public void GetOrAdd_AsyncFactory_Returns_ExistingValue_When_CacheContains_key()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            cacheServiceMock.Setup(x => x.GetAsync<string>(Key)).ReturnsAsync(ExpectedValue);

            // Act
            var result = cacheServiceMock.Object.GetOrAdd(
                Key, 
                () =>
                {
                    throw new InvalidOperationException("Value factory should not be called.");
#pragma warning disable CS0162 // Unreachable code detected
                    return Task.FromResult(string.Empty);
#pragma warning restore CS0162 // Unreachable code detected
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(ExpectedValue, result);
        }

        [Fact]
        public void GetOrAdd_AsyncFactory_Returns_NewValue_When_CacheDoesNotContain_key()
        {
            // Arrange
            var cacheServiceMock = new Mock<ICacheProvider>();
            var valueFactoryCalled = false;

            cacheServiceMock.Setup(x => x.GetAsync<string>(Key))
                .ReturnsAsync((string)null!);

            // Act
            var result = cacheServiceMock.Object.GetOrAdd(Key, () =>
                {
                    valueFactoryCalled = true;
                    return Task.FromResult(ExpectedValue);
                },
                x => !string.IsNullOrEmpty(x),
                null);

            // Assert
            Assert.Equal(ExpectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheServiceMock.Verify(x => x.AddAsync(Key, ExpectedValue, null, false), Times.Once);
        }
    }
}