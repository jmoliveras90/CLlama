using LLama;
using LLama.Common;

namespace CLLama.Infrastructure.Services
{
    public class ModelLoader
    {
        public InteractiveExecutor Load(string modelPath, uint contextSize = 1024, int gpuLayerCount = 5)
        {
            var parameters = new ModelParams(modelPath)
            {
                ContextSize = contextSize,
                GpuLayerCount = gpuLayerCount
            };

            var model = LLamaWeights.LoadFromFile(parameters);
            var context = model.CreateContext(parameters);

            return new InteractiveExecutor(context);
        }
    }
}
