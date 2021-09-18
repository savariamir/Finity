namespace Finity.Address.Abstractions
{
    public interface IQueryParam
    {
        IQueryParam SetQueryParam(string name, params string[] values);
        
        IGenerate EncodeUrl();
        string Generate();
    }
}