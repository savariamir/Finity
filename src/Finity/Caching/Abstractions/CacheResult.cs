namespace Finity.Caching.Abstractions
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