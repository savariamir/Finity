using System;

namespace Finity.Authentication.Configurations
{
    public class AuthenticationConfigure
    {
        public string Endpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public TimeSpan AbsoluteExpirationRelativeToNow { get; set; }
    }
}