namespace Anshan.Integration.Http.Retry
{
    public interface IRetryPolicy
    {
        bool CanRetry();
    }
}