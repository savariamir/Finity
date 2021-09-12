namespace Anshan.Integration.Http.Caching
{
    internal class CacheResult<T>
    {
        public CacheResult(T data)
        {
            Data = data;
        }

        public T Data { get; }

        public bool Hit => Data is not null;
    }
}