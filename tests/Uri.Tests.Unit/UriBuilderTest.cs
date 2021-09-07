using FluentAssertions;
using Uri.Builders;
using Xunit;

namespace Uri.Tests.Unit
{
    public class UriBuilderTest
    {
        [Fact]
        public void Should_generate_a_uri_with_path()
        {
            var uri = UriBuilder
                      .NewUrl()
                      .SetDomain("www.zoomit.ir")
                      .SetPath("mobile")
                      .SetPath("samsung", "galaxy")
                      .Generate();


            uri.Should().Be("www.zoomit.ir/mobile/samsung/galaxy");
        }
        
        [Fact]
        public void Should_generate_a_uri_with_query_params()
        {
            var uri = UriBuilder
                      .NewUrl()
                      .SetDomain("www.zoomit.ir")
                      .SetQueryParam("name1","value1")
                      .SetQueryParam("name2","value2")
                      .Generate();


            uri.Should().Be("www.zoomit.ir?name1=value1&name2=value2");
        }
        
        [Fact]
        public void Should_generate_a_uri_with_path_and_query_params()
        {
            var uri = UriBuilder
                      .NewUrl()
                      .SetDomain("www.zoomit.ir")
                      .SetPath("mobile")
                      .SetPath("samsung", "galaxy")
                      .SetQueryParam("name1","value1")
                      .SetQueryParam("name2","value2")
                      .Generate();


            uri.Should().Be("www.zoomit.ir/mobile/samsung/galaxy?name1=value1&name2=value2");
        }
        
        
        [Fact]
        public void Should_generate_a_uri_with_query_params_array()
        {
            var uri = UriBuilder
                      .NewUrl()
                      .SetDomain("www.zoomit.ir")
                      .SetQueryParam("name1", "1","2","3")
                      .Generate();


            uri.Should().Be("www.zoomit.ir?name1=1,2,3");
        }
    }
}