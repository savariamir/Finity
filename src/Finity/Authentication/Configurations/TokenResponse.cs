using System.Text.Json.Serialization;

namespace Finity.Authentication.Configurations
{
    public class TokenResponse
    {
        public TokenResponse(string accessToken, int expiresIn, string tokenType, string scope)
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            TokenType = tokenType;
            Scope = scope;
        }
        [JsonPropertyName("access_token")]
        public string AccessToken { get; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; }
        [JsonPropertyName("scope")]
        public string Scope { get; }
    }
}