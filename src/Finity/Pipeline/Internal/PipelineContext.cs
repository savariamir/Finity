using Finity.Pipeline.Abstractions;

namespace Finity.Pipeline.Internal
{
    public class PipelineContext : IPipelineContext
    {
        public PipelineContext()
        {
            Data = new PipelineContextData();
        }

        public IPipelineContextData Data { get; }
    }
}