namespace IntegrationMocks.Core.Tests.Fixtures;

public static class EnumerableExtensions
{
    private static readonly Random Random = new();

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(_ => Random.Next());
    }
}
