namespace Anshan.Integration.Http.Http.Caching
{
    public static class CacheKey
    {
        private const string Suffix = "Anshan.Integration";
        public static string GetKey(string url) => $"{Suffix}:{url}";
    }
}