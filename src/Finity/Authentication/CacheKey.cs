namespace Finity.Authentication
{
    internal static class CacheKey
    {
        private const string Suffix = "finity:token";
        public static string GetKey(string clientName) => $"{Suffix}:{clientName}";
    }
}