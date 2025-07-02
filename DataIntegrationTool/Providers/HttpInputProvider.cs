using DataIntegrationTool.Config;
using DataIntegrationTool.Providers.Interfaces;
using DataIntegrationTool.Services;

namespace DataIntegrationTool.Providers
{
    public class HttpInputProvider(InputSourceConfig config) : IInputProvider
    {
        public async Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class
        {
            using var httpClient = new HttpClient();
            var data = await httpClient.GetStreamAsync(config.Url);

            return await new CsvReaderService().ReadCsvAsync<T>(data, config.Options, config.Encoding);
        }
    }
}
