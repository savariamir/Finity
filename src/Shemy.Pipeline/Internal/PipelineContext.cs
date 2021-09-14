using Shemy.Pipeline.Abstractions;

namespace Shemy.Pipeline.Internal
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