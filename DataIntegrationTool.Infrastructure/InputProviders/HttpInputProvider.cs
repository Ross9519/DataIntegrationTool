using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Infrastructure.InputProviders
{
    public class HttpInputProvider(ICsvReaderService csvService) : IInputProvider
    {
        private InputSourceConfig _config = default!;

        public async Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class
        {
            using var httpClient = new HttpClient();
            var data = await httpClient.GetStreamAsync(_config.Url);

            return await csvService.ReadCsvAsync<T>(data, _config.Options, _config.Encoding);
        }

        public HttpInputProvider WithConfig(InputSourceConfig config)
        {
            _config = config;
            return this;
        }
    }
}
