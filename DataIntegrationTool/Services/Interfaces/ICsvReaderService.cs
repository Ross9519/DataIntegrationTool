
using DataIntegrationTool.Config;

namespace DataIntegrationTool.Services.Interfaces
{
    public interface ICsvReaderService
    {
        Task<IEnumerable<T>> ReadCsvAsync<T>(Stream csvStream, CsvReaderOptionsConfig? options, string encoding) where T : class;
    }
}
