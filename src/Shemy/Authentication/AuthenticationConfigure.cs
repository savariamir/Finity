using System;

namespace Shemy.Authentication
{
    public class AuthenticationConfigure
    {
        public string Address { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public TimeSpan AbsoluteExpirationRelativeToNow { get; set; }
        
    }
}