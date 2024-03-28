namespace IL.InMemoryCacheProvider.Options;

public sealed class ExpirationOptions
{
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; init; }
    public DateTimeOffset? AbsoluteExpiration { get; init; }
    public TimeSpan? SlidingExpiration { get; init; }
}