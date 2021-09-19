[![.NET 5 CI](https://github.com/savariamir/Finity/actions/workflows/dotnet.yml/badge.svg)](https://github.com/savariamir/Finity/actions/workflows/dotnet.yml)

# Fault tolerance library designed for .Net core

 Finity is a .NET  Core resilience and Fault tolerance library that allows developers to extend IHttpClientFactory such as Retry, Circuit Breaker, Caching, Authentication and, Bulkhead Isolation.

Finity is a lightweight fault tolerance library designed to isolate access to remote resources and services. In a distributed environment, calls to remote resources and services can fail due to transient faults, such as slow network connections, timeouts, or the resources being overcommitted or temporarily unavailable.

# Using Finity with HttpClient factory from ASPNET Core
Finity extends .Net Core HttpClient Factory to avoid transienting faults.

# Retry

```c#
services
    .AddHttpClient("finity")
    .WithRetry(options =>
    {
        options.SleepDurationRetry = TimeSpan.FromMilliseconds(100);
        options.RetryCount = 3;
    });
```

# Circuit Breaker

```c#
services
    .AddHttpClient("finity")
    .WithCircuitBreaker(options =>
    {
        options.SuccessAllowedBeforeClosing = 1;
        options.DurationOfBreak = TimeSpan.FromMilliseconds(100);
        options.ExceptionsAllowedBeforeBreaking = 2;
    });
```

# Caching

```c#
services
    .AddHttpClient("finity")
    .WithCache(options =>
    {
        options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
    });
```

# Bulkhead

```c#
services
    .AddHttpClient("finity")
    .WithBulkhead(options =>
    {
        options.MaxConcurrentCalls = 100;
    });
```
