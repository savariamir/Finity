using Microsoft.Extensions.DependencyInjection;

namespace Finity.Extension
{
    public class FinityHttpClientBuilder : IFinityHttpClientBuilder
    {
        public string Name { get; }
        public IServiceCollection Services { get; }

        public FinityHttpClientBuilder(string name, IServiceCollection services)
        {
            Name = name;
            Services = services;
        }
    }
}