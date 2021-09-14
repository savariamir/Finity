namespace Shemy.Http.Caching
{
    internal static class CacheKey
    {
        private const string Suffix = "Anshan.Integration";
        public static string GetKey(string url) => $"{Suffix}:{url}";
    }
}