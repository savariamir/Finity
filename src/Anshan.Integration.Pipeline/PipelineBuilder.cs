using System;
using System.Collections.Generic;
using Anshan.Integration.Pipeline.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Anshan.Integration.Pipeline
{
    public class PipelineBuilder<TRequest, TResponse>
    {
        private readonly IServiceCollection _services;

        internal List<Type> Middlewares { get; }
        
        public PipelineBuilder(IServiceCollection services)
        {
            _services = services;
            Middlewares = new();
        }

        public PipelineBuilder<TRequest, TResponse> AddMiddleware<TMiddleware>()
            where TMiddleware : class, IMiddleware<TRequest, TResponse>
        {
            _services.AddTransient<TMiddleware>();
            Middlewares.Add(typeof(TMiddleware));

            return this;
        }
    }
    
    public class PipelineBuilder<TResponse>
    {
        private readonly IServiceCollection _services;

        internal List<Type> Middlewares { get; }

        public PipelineBuilder(IServiceCollection services)
        {
            _services = services;
            Middlewares = new();
        }

        public PipelineBuilder<TResponse> WithMiddleware<TMiddleware>()
            where TMiddleware : class, IMiddleware<TResponse>
        {
            _services.AddTransient<TMiddleware>();
            Middlewares.Add(typeof(TMiddleware));

            return this;
        }
    }
}