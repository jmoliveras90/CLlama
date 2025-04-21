using LLama;

namespace CLLama.Infrastructure.Services
{
    public class ModelExecutorService
    {
        private InteractiveExecutor? _executor;

        public InteractiveExecutor? Executor => _executor;

        public bool IsModelLoaded => _executor != null;

        public void SetExecutor(InteractiveExecutor executor)
        {
            _executor = executor;
        }

        public void ClearExecutor()
        {
            _executor = null;
        }
    }
}
