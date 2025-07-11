using System.Text;
using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Infrastructure.InputProviders
{
    public class StringInputProvider(ICsvReaderService csvService) : InputProviderBase<StringInputProvider>
    {
        public override async Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class
        {
            if (string.IsNullOrWhiteSpace(_config.CsvStringContent))
                throw new ArgumentNullException(nameof(_config.CsvStringContent), "CSV string content cannot be null or empty.");

            var bytes = Encoding.GetEncoding(_config.Encoding).GetBytes(_config.CsvStringContent);
            return await csvService.ReadCsvAsync<T>(new MemoryStream(bytes), _config.Options, _config.Encoding);
        }
    }
}
