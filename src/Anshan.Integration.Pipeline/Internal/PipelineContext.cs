using Anshan.Integration.Pipeline.Abstractions;

namespace Anshan.Integration.Pipeline.Internal
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