using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Infrastructure.InputProviders
{
    public class FileInputProvider(ICsvReaderService csvService, IFileReader fileReader) : InputProviderBase<FileInputProvider>
    {
        public override async Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class
        {
            if (string.IsNullOrWhiteSpace(_config.FilePath))
                throw new ArgumentNullException(nameof(_config.FilePath), "File path cannot be null or empty.");

            if (!fileReader.Exists(_config.FilePath))
                throw new FileNotFoundException("File not found.", _config.FilePath);

            using var stream = fileReader.OpenRead(_config.FilePath!);
            var prova = csvService.ReadCsvAsync<T>(stream, _config.Options, _config.Encoding);
            return await prova;
        }
    }
}
