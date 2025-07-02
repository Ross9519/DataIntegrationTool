using System.Text;
using DataIntegrationTool.Config;
using DataIntegrationTool.Providers.Interfaces;
using DataIntegrationTool.Services;

namespace DataIntegrationTool.Providers
{
    public class StringInputProvider(InputSourceConfig config) : IInputProvider
    {
        public async Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class
        {
            var bytes = Encoding.GetEncoding(config.Encoding).GetBytes(config.CsvStringContent!);
            return await new CsvReaderService().ReadCsvAsync<T>(new MemoryStream(bytes), config.Options, config.Encoding);
        }
    }
}
