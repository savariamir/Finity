using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.Pipeline.Internal;
using Finity.Request;
using Finity.Retry.Configurations;
using Finity.Retry.Exceptions;
using Finity.Retry.Internals;
using Finity.Shared;
using Finity.Shared.Metrics;
using Finity.Tests.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Finity.Tests.Retry
{
    public class RetryMiddlewareTest
    {
        [Fact]
        public async Task Should_success_for_the_first_try()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RetryMiddleware>>();
            var clock =new FakeSystemClock();
            var metricProvider = Substitute.For<IMetricProvider>();
            var options = Substitute.For<IOptionsSnapshot<RetryConfigure>>();
            var middleware = new RetryMiddleware(clock, options,logger, metricProvider);
            var cancellationToken = new CancellationToken();
            var context = new PipelineContext();
            var request = new FinityHttpRequestMessage
            {
                Name = "finity",
                HttpRequest = new HttpRequestMessage(HttpMethod.Post, "test.com")
            };
            var nextMock = new FakeNextMiddleware();
            
            //Act
            await middleware.ExecuteAsync(request, context, nextMock.SuccessNext, cancellationToken);

            //Assert
            nextMock.SuccessCallsCount.Should().Be(1);
        }
        
        [Fact]
        public async Task Should_add_metric_for_the_first_try()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RetryMiddleware>>();
            var clock =new FakeSystemClock();
            var metricProvider = Substitute.For<IMetricProvider>();
            var options = Substitute.For<IOptionsSnapshot<RetryConfigure>>();
            var middleware = new RetryMiddleware(clock, options,logger, metricProvider);
            var cancellationToken = new CancellationToken();
            var context = new PipelineContext();
            var request = new FinityHttpRequestMessage
            {
                Name = "finity",
                HttpRequest = new HttpRequestMessage(HttpMethod.Post, "test.com")
            };
            var nextMock = new FakeNextMiddleware();
            
            //Act
            await middleware.ExecuteAsync(request, context, nextMock.SuccessNext, cancellationToken);

            //Assert
            metricProvider.Received(1).AddMetric(Arg.Any<CounterValue>());
        }
        
        [Fact]
        public async Task Should_retry_after_failing_the_first_try_and_throw_Exception()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RetryMiddleware>>();
            var clock =new FakeSystemClock();
            var metricProvider = Substitute.For<IMetricProvider>();
            var options = Substitute.For<IOptionsSnapshot<RetryConfigure>>();
            options.Get("finity").Returns(new RetryConfigure {RetryCount = 5, SleepDurationRetry = TimeSpan.Zero});
            var middleware = new RetryMiddleware(clock, options,logger, metricProvider);
            var cancellationToken = new CancellationToken();
            var context = new PipelineContext();
            var request = new FinityHttpRequestMessage
            {
                Name = "finity",
                HttpRequest = new HttpRequestMessage(HttpMethod.Post, "test.com")
            };
            var nextMock = new FakeNextMiddleware();
            
            //Act
           Func<Task<HttpResponseMessage>> act= async () => await middleware.ExecuteAsync(request, context, nextMock.FailureNext, cancellationToken);

           act.Should().Throw<RetryOutOfRangeException>();
            //Assert
            nextMock.FailureCallsCount.Should().Be(6);
        }
        
        [Fact]
        public async Task Should_success_after_some_retries()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RetryMiddleware>>();
            var clock =new FakeSystemClock();
            var metricProvider = Substitute.For<IMetricProvider>();
            var options = Substitute.For<IOptionsSnapshot<RetryConfigure>>();
            options.Get("finity").Returns(new RetryConfigure {RetryCount = 5, SleepDurationRetry = TimeSpan.FromSeconds(1)});
            var middleware = new RetryMiddleware(clock, options,logger, metricProvider);
            var cancellationToken = new CancellationToken();
            var context = new PipelineContext();
            var request = new FinityHttpRequestMessage
            {
                Name = "finity",
                HttpRequest = new HttpRequestMessage(HttpMethod.Post, "test.com")
            };
            var nextMock = new FakeNextMiddleware().SuccessNextAfter(3);
            
            //Act
            var response = await middleware.ExecuteAsync(request, context, nextMock.SuccessNextAfterSomeRetries, cancellationToken);
            
            //Assert
            response.Should().BeEquivalentTo(new HttpResponseMessage(HttpStatusCode.OK));
            nextMock.FailureCallsCount.Should().Be(4);
            nextMock.SuccessCallsCount.Should().Be(1);
        }
        
        [Fact]
        public async Task Should_add_metric_after_some_retries()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RetryMiddleware>>();
            var clock =new FakeSystemClock();
            var metricProvider = Substitute.For<IMetricProvider>();
            var options = Substitute.For<IOptionsSnapshot<RetryConfigure>>();
            options.Get("finity").Returns(new RetryConfigure {RetryCount = 5, SleepDurationRetry = TimeSpan.FromSeconds(1)});
            var middleware = new RetryMiddleware(clock, options,logger, metricProvider);
            var cancellationToken = new CancellationToken();
            var context = new PipelineContext();
            var request = new FinityHttpRequestMessage
            {
                Name = "finity",
                HttpRequest = new HttpRequestMessage(HttpMethod.Post, "test.com")
            };
            var nextMock = new FakeNextMiddleware().SuccessNextAfter(3);
            
            //Act
            await middleware.ExecuteAsync(request, context, nextMock.SuccessNextAfterSomeRetries, cancellationToken);
            
            //Assert
            metricProvider.Received(1).AddMetric(Arg.Any<CounterValue>());
        }
        
        [Fact]
        public async Task Should_sleep_between_retries()
        {
            //Arrange
            var logger = Substitute.For<ILogger<RetryMiddleware>>();
            var clock =new FakeSystemClock();
            var metricProvider = Substitute.For<IMetricProvider>();
            var options = Substitute.For<IOptionsSnapshot<RetryConfigure>>();
            options.Get("finity").Returns(new RetryConfigure {RetryCount = 5, SleepDurationRetry = TimeSpan.FromSeconds(1)});
            var middleware = new RetryMiddleware(clock, options,logger, metricProvider);
            var cancellationToken = new CancellationToken();
            var context = new PipelineContext();
            var request = new FinityHttpRequestMessage
            {
                Name = "finity",
                HttpRequest = new HttpRequestMessage(HttpMethod.Post, "test.com")
            };
            var nextMock = new FakeNextMiddleware().SuccessNextAfter(3);
            
            //Act
            await middleware.ExecuteAsync(request, context, nextMock.SuccessNextAfterSomeRetries, cancellationToken);
            
            //Assert
            clock.SleepCallsCount.Should().Be(3);
        }
    }
}