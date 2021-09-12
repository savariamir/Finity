using System;
using System.Collections.Generic;
using Anshan.Integration.Pipeline.Abstractions;

namespace Anshan.Integration.Pipeline.Internal
{
    internal class PipelineContextData : IPipelineContextData
    {
        private readonly Dictionary<Type, object> _data = new();


        public T Get<T>() => (T) _data[typeof(T)];

        public void Set<T>(T instance) => _data[typeof(T)] = instance;
    }
}