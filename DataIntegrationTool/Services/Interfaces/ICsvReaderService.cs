
namespace DataIntegrationTool.Services.Interfaces
{
    public interface ICsvReaderService
    {
        Task<IEnumerable<T>> HandleStreamAsync<T>(Stream csvStream, CsvReaderOptions? options = null) where T : class;
        Task<IEnumerable<T>> HandleContentAsync<T>(string? csvContent, CsvReaderOptions? options = null) where T : class;

        Task<IEnumerable<T>> HandleFilePathAsync<T>(string filePath, CsvReaderOptions? options = null) where T : class;
    }
}
