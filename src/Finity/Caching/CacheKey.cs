namespace Finity.Caching
{
    internal static class CacheKey
    {
        private const string Suffix = "finity";
        public static string GetKey(string url) => $"{Suffix}:{url}";
    }
}