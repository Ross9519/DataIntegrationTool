using static DataIntegrationTool.Shared.Utils.CsvEnums;
using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Application.Factories
{
    public class InputProviderFactory(IReadOnlyDictionary<InputType, InputProviderResolver> resolvers)
    {
        public IInputProvider Create(InputSourceConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);

            if (resolvers.TryGetValue(config.Type, out var resolver))
            {
                return resolver(config);
            }

            throw new InvalidOperationException("Input type non supportato");
        }
    }
}
