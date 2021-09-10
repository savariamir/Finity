using System;
using Anshan.Integration.Http.AddressBuilder.Builders;
using Anshan.Integration.Http.AddressBuilder.Exceptions;
using FluentAssertions;
using Xunit;

namespace Uri.Tests.Unit
{
    public class UriBuilderTest
    {
        [Fact]
        public void Should_generate_a_uri_with_path()
        {
            var uri = AddressBuilder
                .Create()
                .SetBaseAddress("https://www.example.com")
                .SetPath("mobile")
                .SetPath("samsung", "galaxy")
                .Generate();


            uri.Should().Be("https://www.example.com/mobile/samsung/galaxy");
        }

        [Fact]
        public void Should_generate_a_uri_with_query_params()
        {
            var uri = AddressBuilder
                .Create()
                .SetBaseAddress("https://www.example.com")
                .SetQueryParam("name1", "value1")
                .SetQueryParam("name2", "value2")
                .Generate();


            uri.Should().Be("https://www.example.com?name1=value1&name2=value2");
        }

        [Fact]
        public void Should_generate_a_uri_with_path_and_query_params()
        {
            var uri = AddressBuilder
                .Create()
                .SetBaseAddress("https://www.example.com")
                .SetPath("mobile")
                .SetPath("samsung", "galaxy")
                .SetQueryParam("name1", "value1")
                .SetQueryParam("name2", "value2")
                .Generate();


            uri.Should().Be("https://www.example.com/mobile/samsung/galaxy?name1=value1&name2=value2");
        }


        [Fact]
        public void Should_generate_a_uri_with_query_params_array()
        {
            var uri = AddressBuilder
                .Create()
                .SetBaseAddress("https://www.example.com")
                .SetQueryParam("name1", "1", "2", "3")
                .Generate();


            uri.Should().Be("https://www.example.com?name1=1,2,3");
        }

        [Theory]
        [InlineData("example.com")]
        [InlineData("example")]
        [InlineData("ww.example.com")]
        [InlineData("www.example")]
        public void Should_throw_AbsoluteUrlException_when_domain_is_not_correct(string domain)
        {
            Action func = () =>
                AddressBuilder
                    .Create()
                    .SetBaseAddress(domain)
                    .Generate();

            func.Should().Throw<AbsoluteUrlException>();
        }
    }
}