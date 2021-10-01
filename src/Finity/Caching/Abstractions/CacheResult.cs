namespace Finity.Caching.Abstractions
{
    public class CacheResult<T>
    {
        public CacheResult(T data)
        {
            Data = data;
        }

        public T Data { get; }

        public bool Hit => Data is not null;
        public bool Miss => Data is null;
    }
}