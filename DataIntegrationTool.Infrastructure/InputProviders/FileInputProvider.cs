using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Infrastructure.InputProviders
{
    public class FileInputProvider(ICsvReaderService csvService) : IInputProvider
    {
        private InputSourceConfig _config = default!;

        public async Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class
        {
            using var fileStream = new FileStream(_config.FilePath!, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await csvService.ReadCsvAsync<T>(fileStream, _config.Options, _config.Encoding);
        }

        public FileInputProvider WithConfig(InputSourceConfig config)
        {
            _config = config;
            return this;
        }
    }
}
