using static DataIntegrationTool.Utils.CsvEnums;
using DataIntegrationTool.Config;
using static DataIntegrationTool.Utils.Constants;
using DataIntegrationTool.Providers.Interfaces;

namespace DataIntegrationTool.Providers.Factories
{
    public static class InputProviderFactory
    {
        public static IInputProvider Create(InputSourceConfig config)
        {
            return config.Type switch
            {
                InputType.File when config.FilePath is not null =>
                    new FileInputProvider(config),

                InputType.Http when config.Url is not null =>
                    new HttpInputProvider(config),

                InputType.String when config.CsvStringContent is not null =>
                    new StringInputProvider(config),

                InputType.Database when config.ConnectionString is not null && config.Query is not null =>
                    new DatabaseInputProvider(config),

                _ => throw new InvalidOperationException($"{INPUT} {ERRORMESSAGEPROGRAM}")
            };
        }
    }
}
