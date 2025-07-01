namespace DataIntegrationTool.Services.Interfaces
{
    public interface ICsvReaderService
    {
        Task<IEnumerable<T>> ConvertFromCsvToTAsync<T>(string csvContent) where T : class;

        Task<IEnumerable<T>> ReadFromFileAsync<T>(string filePath) where T : class;
    }
}
