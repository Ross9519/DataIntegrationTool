using DataIntegrationTool.Config;
using DataIntegrationTool.Providers.Interfaces;
using DataIntegrationTool.Services;

namespace DataIntegrationTool.Providers
{
    public class FileInputProvider(InputSourceConfig config) : IInputProvider
    {
        public async Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class
        {
            using var fileStream = new FileStream(config.FilePath!, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await new CsvReaderService().ReadCsvAsync<T>(fileStream, config.Options, config.Encoding);
        }
    }
}
