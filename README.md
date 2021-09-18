# Fault tolerance library designed for .Net core

Finity is a .NET resilience and transient-fault-handling library that allows developers to extend IHttpClientFactory such as Retry, Circuit Breaker, Caching, Authentication and, Bulkhead.

Finity is a lightweight fault tolerance library designed to isolate access to remote resources and services. In a distributed environment, calls to remote resources and services can fail due to transient faults, such as slow network connections, timeouts, or the resources being overcommitted or temporarily unavailable.

# Using Finity with HttpClient factory from ASPNET Core
Finity extends .Net Core HttpClient Factory to avoid transienting faults.

# Retry

```c#
services
    .AddFinityHttpClient("finity")
    .AddRetry(options =>
    {
        options.SleepDurationRetry = TimeSpan.FromMilliseconds(100);
        options.RetryCount = 3;
    });
```

# Circuit Breaker

```c#
services
    .AddFinityHttpClient("finity")
    .AddCircuitBreaker(options =>
    {
        options.SuccessAllowedBeforeClosing = 1;
        options.DurationOfBreak = TimeSpan.FromMilliseconds(100);
        options.ExceptionsAllowedBeforeBreaking = 2;
    });
```

# Caching

```c#
services
    .AddFinityHttpClient("finity")
    .AddCache(options =>
    {
        options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
    });
```

# Bulkhead

```c#
services
    .AddFinityHttpClient("finity")
    .AddBulkhead(options =>
    {
        options.MaxConcurrentCalls = 100;
    });
```
