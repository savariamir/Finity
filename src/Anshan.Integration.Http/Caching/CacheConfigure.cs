using System;

namespace Anshan.Integration.Http.Caching
{
    public class CacheConfigure
    {
        public TimeSpan AbsoluteExpirationRelativeToNow { set; get; }
    }
}