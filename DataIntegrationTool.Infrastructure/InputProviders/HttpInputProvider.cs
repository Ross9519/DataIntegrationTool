using System.Text;
using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Infrastructure.InputProviders
{
    public class HttpInputProvider(ICsvReaderService csvService, HttpClient httpClient = null!) : InputProviderBase<HttpInputProvider>
    {
        private readonly HttpClient? _httpClient = httpClient ?? new HttpClient();

        public override async Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class
        {
            if (string.IsNullOrWhiteSpace(_config.Url))
                throw new ArgumentNullException(nameof(_config.Url), "URL cannot be null or empty.");

            try
            {
                Encoding.GetEncoding(_config.Encoding);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Invalid encoding: {_config.Encoding}", ex);
            }

            var data = await _httpClient!.GetStreamAsync(_config.Url);

            return await csvService.ReadCsvAsync<T>(data, _config.Options, _config.Encoding);
        }
    }
}
