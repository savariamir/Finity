namespace Shemy.Caching
{
    internal static class CacheKey
    {
        private const string Suffix = "shemy";
        public static string GetKey(string url) => $"{Suffix}:{url}";
    }
}