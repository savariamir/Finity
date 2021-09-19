using System;
using System.Collections.Generic;

namespace Finity.Pipeline
{
    public class AnshanFactoryOptions
    {
        public HashSet<Type> Types { get; set; } = new();
    }
}