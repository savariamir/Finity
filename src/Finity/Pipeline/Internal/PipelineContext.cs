using Finity.Pipeline.Abstractions;

namespace Finity.Pipeline.Internal
{
    internal class PipelineContext : IPipelineContext
    {
        public PipelineContext()
        {
            Data = new PipelineContextData();
        }

        public IPipelineContextData Data { get; }
    }
}