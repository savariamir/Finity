using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Shemy.Pipeline.Abstractions;

namespace Shemy.Pipeline
{
    public class PipelineBuilder<TRequest, TResponse>
    {
        private readonly IServiceCollection _services;

        public List<Type> Middlewares { get; }
        
        public PipelineBuilder(IServiceCollection services)
        {
            _services = services;
            Middlewares = new List<Type>();
        }

        public PipelineBuilder<TRequest, TResponse> AddMiddleware<TMiddleware>()
            where TMiddleware : class, IMiddleware<TRequest, TResponse>
        {
            _services.AddTransient<TMiddleware>();
            Middlewares.Add(typeof(TMiddleware));

            return this;
        }
    }
}