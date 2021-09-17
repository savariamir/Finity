namespace Shemy.Authentication
{
    internal static class CacheKey
    {
        private const string Suffix = "shemy:token";
        public static string GetKey(string clientName) => $"{Suffix}:{clientName}";
    }
}