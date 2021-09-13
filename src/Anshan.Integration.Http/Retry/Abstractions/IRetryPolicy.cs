namespace Anshan.Integration.Http.Retry.Abstractions
{
    internal interface IRetryPolicy
    {
        bool CanRetry();
    }
}