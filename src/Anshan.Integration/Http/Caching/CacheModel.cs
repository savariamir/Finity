namespace Anshan.Integration.Http.Caching
{
    public class CacheModel<T>
    {
        public CacheModel(T data)
        {
            Data = data;
        }

        public T Data { get; }

        public bool Hit => Data is not null;
    }
}