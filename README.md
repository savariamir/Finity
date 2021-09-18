# Fault tolerance library designed for .Net core

Finity is a lightweight fault tolerance library designed to isolate access to remote resources and services. In a distributed environment, calls to remote resources and services can fail due to transient faults, such as slow network connections, timeouts, or the resources being overcommitted or temporarily unavailable.

# Retry

```c#
services.AddShemyHttpClient("finity")
    .AddRetry(options =>
    {
        options.SleepDurationRetry = TimeSpan.FromMilliseconds(100);
        options.RetryCount = 3;
    });
```
