namespace Anshan.Integration.Http.Http.Retry
{
    public interface IRetryPolicy
    {
        bool CanRetry();
    }
}