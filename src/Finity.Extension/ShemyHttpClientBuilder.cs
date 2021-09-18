using Microsoft.Extensions.DependencyInjection;

namespace Finity.Extension
{
    public class ShemyHttpClientBuilder : IShemyHttpClientBuilder
    {
        public string Name { get; }
        public IServiceCollection Services { get; }

        public ShemyHttpClientBuilder(string name, IServiceCollection services)
        {
            Name = name;
            Services = services;
        }
    }
}