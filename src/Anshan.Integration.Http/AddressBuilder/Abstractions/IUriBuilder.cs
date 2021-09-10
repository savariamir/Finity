namespace Anshan.Integration.Http.AddressBuilder.Abstractions
{
    public interface IUriBuilder
    {
        IPath SetBaseAddress(string value);
    }
}