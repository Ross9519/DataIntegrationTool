using System.Text;
using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Infrastructure.InputProviders
{
    public class StringInputProvider(ICsvReaderService csvService) : IInputProvider
    {
        private InputSourceConfig _config = default!;

        public async Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class
        {
            var bytes = Encoding.GetEncoding(_config.Encoding).GetBytes(_config.CsvStringContent!);
            return await csvService.ReadCsvAsync<T>(new MemoryStream(bytes), _config.Options, _config.Encoding);
        }

        public StringInputProvider WithConfig(InputSourceConfig config)
        {
            _config = config;
            return this;
        }
    }
}
