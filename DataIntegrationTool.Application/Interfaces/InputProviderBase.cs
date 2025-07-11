using DataIntegrationTool.Application.Config;

namespace DataIntegrationTool.Application.Interfaces
{
    public abstract class InputProviderBase<TProvider> : IInputProvider where TProvider : InputProviderBase<TProvider>
    {
        protected InputSourceConfig _config = default!;

        public TProvider WithConfig(InputSourceConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            return (TProvider)(object)this;
        }

        public abstract Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class;
    }
}
