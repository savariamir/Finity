using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.Caching.Abstractions;
using Finity.Caching.Internals;
using Finity.Pipeline.Internal;
using Finity.Request;
using Finity.Shared;
using Finity.Shared.Metrics;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Finity.Tests.Cache
{
    public class MemoryCacheMiddlewareTest
    {
        [Fact]
        public async Task Should_pass_when_http_method_is_not_get()
        {
            //Arrange
            var cacheProvider = Substitute.For<IMemoryCacheProvider>();
            var logger = Substitute.For<ILogger<MemoryCacheMiddleware>>();
            var metricProvider = Substitute.For<IMetricProvider>();
            var middleware = new MemoryCacheMiddleware(cacheProvider, logger, metricProvider);
            var cancellationToken = new CancellationToken();
            var context = new PipelineContext();
            var request = new FinityHttpRequestMessage
            {
                Name = "finity",
                HttpRequest = new HttpRequestMessage(HttpMethod.Post, "test.com")
            };
            var nextMock = new NextMock();
            
            //Act
            await middleware.ExecuteAsync(request, context, nextMock.Next, cancellationToken);

            //Assert
            nextMock.CallsCount.Should().Be(1);
        }

        [Fact]
        public async Task Should_miss_cache_for_the_first_time_when_http_method_is_get()
        {
            //Arrange
            var cacheProvider = Substitute.For<IMemoryCacheProvider>();
            cacheProvider.GetFromCache("test.com")
                .Returns(new CacheResult<HttpResponseMessage>(null));
            var logger = Substitute.For<ILogger<MemoryCacheMiddleware>>();
            var metricProvider = Substitute.For<IMetricProvider>();
            var middleware = new MemoryCacheMiddleware(cacheProvider, logger, metricProvider);
            var cancellationToken = new CancellationToken();
            var context = new PipelineContext();
            var request = new FinityHttpRequestMessage
            {
                Name = "finity",
                HttpRequest = new HttpRequestMessage(HttpMethod.Get, "test.com")
            };
            var nextMock = new NextMock();

            //Act
            var response = await middleware.ExecuteAsync(request, context, nextMock.Next, cancellationToken);

            //Assert
            nextMock.CallsCount.Should().Be(1);
            response.Should().BeEquivalentTo(new HttpResponseMessage(HttpStatusCode.OK));
            cacheProvider.Received(1).SetToCache(request, nextMock.Response);
        }


        [Fact]
        public async Task Should_hit_cache_for_the_second_time_and_not_execute_next_when_http_method_is_get()
        {
            //Arrange
            var cacheProvider = Substitute.For<IMemoryCacheProvider>();
            cacheProvider.GetFromCache("test.com")
                .Returns(new CacheResult<HttpResponseMessage>(new HttpResponseMessage()));

            var logger = Substitute.For<ILogger<MemoryCacheMiddleware>>();
            var metricProvider = Substitute.For<IMetricProvider>();
            var middleware = new MemoryCacheMiddleware(cacheProvider, logger, metricProvider);
            var cancellationToken = new CancellationToken();
            var context = new PipelineContext();
            var request = new FinityHttpRequestMessage
            {
                Name = "finity",
                HttpRequest = new HttpRequestMessage(HttpMethod.Get, "test.com")
            };
            var nextMock = new NextMock();

            //Act
            var response = await middleware.ExecuteAsync(request, context, nextMock.Next, cancellationToken);

            //Assert
            nextMock.CallsCount.Should().Be(0);
            response.Should().BeEquivalentTo(new HttpResponseMessage(HttpStatusCode.OK));
            cacheProvider.Received(0).SetToCache(request, nextMock.Response);
        }

        [Fact]
        public async Task Should_add_metric_when_cache_hit()
        {
            //Arrange
            var cacheProvider = Substitute.For<IMemoryCacheProvider>();
            cacheProvider.GetFromCache("test.com")
                .Returns(new CacheResult<HttpResponseMessage>(new HttpResponseMessage()));

            var logger = Substitute.For<ILogger<MemoryCacheMiddleware>>();
            var metricProvider = Substitute.For<IMetricProvider>();
            var middleware = new MemoryCacheMiddleware(cacheProvider, logger, metricProvider);
            var cancellationToken = new CancellationToken();
            var context = new PipelineContext();
            var request = new FinityHttpRequestMessage
            {
                Name = "finity",
                HttpRequest = new HttpRequestMessage(HttpMethod.Get, "test.com")
            };
            var nextMock = new NextMock();

            //Act
             await middleware.ExecuteAsync(request, context, nextMock.Next, cancellationToken);

             //Assert
            metricProvider.Received(1).AddMetric(Arg.Any<CounterValue>());
        }
    }
}