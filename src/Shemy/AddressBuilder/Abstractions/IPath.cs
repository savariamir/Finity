namespace Shemy.AddressBuilder.Abstractions
{
    public interface IPath
    {
        IPath SetPath(params string[] path);
        IQueryParam SetQueryParam(string name, params string[] values);
        string Generate();
        IGenerate EncodeUrl();
    }
}