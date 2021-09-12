namespace Anshan.Integration.Http.Retry.Appstractions
{
    internal interface IRetryPolicy
    {
        bool CanRetry();
    }
}