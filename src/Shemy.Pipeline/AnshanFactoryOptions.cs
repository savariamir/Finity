using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Shemy.Pipeline
{
    public class AnshanFactoryOptions
    {
        public List<Type> Types { get; set; } = new();
        public readonly ConcurrentDictionary<string, SemaphoreSlim> SemaphoreSlims = new();
    }
}