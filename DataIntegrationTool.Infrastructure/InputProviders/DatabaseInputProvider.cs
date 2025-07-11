using System.Text;
using DataIntegrationTool.Application.Factories;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Infrastructure.DataAccess;
using DataIntegrationTool.Shared.Utils;

namespace DataIntegrationTool.Infrastructure.InputProviders
{
    public class DatabaseInputProvider(ICsvReaderService csvService, IDatabaseAccessor dbAccessor) : InputProviderBase<DatabaseInputProvider>
    {
        public override async Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class
        {
            if (string.IsNullOrWhiteSpace(_config.ConnectionString))
                throw new ArgumentNullException(nameof(_config.ConnectionString), "ConnectionString cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(_config.Query))
                throw new ArgumentNullException(nameof(_config.Query), "Query cannot be null or empty.");

            using var reader = await dbAccessor.ExecuteReaderAsync(_config.Query);

            var encodingObj = Encoding.GetEncoding(_config.Encoding);
            using var csvStream = new DbDataReaderCsvStream(reader, encodingObj, _config.Options?.Delimiter.ToString() ?? ",");

            return await csvService.ReadCsvAsync<T>(csvStream, _config.Options, _config.Encoding);
        }
    }
}
