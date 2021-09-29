using Xunit;

namespace Finity.Tests.Cache
{
    public class MemoryCacheMiddlewareTest
    {
        [Fact]
        public void Should_pass_when_http_method_is_not_get()
        {
        }
        
        [Fact]
        public void Should_miss_cache_for_the_first_time_when_http_method_is_get()
        {
        }
        
        [Fact]
        public void Should_hit_cache_for_second_time_when_http_method_is_get()
        {
        }

        [Fact]
        public void Should_add_metric_when_cache_hit()
        {
        }
    }
}