using System.Text;
using DataIntegrationTool.Config;
using DataIntegrationTool.Providers.Interfaces;
using DataIntegrationTool.Services;
using DataIntegrationTool.Utils;
using Microsoft.Data.SqlClient;

namespace DataIntegrationTool.Providers
{
    public class DatabaseInputProvider(InputSourceConfig config) : IInputProvider
    {
        public async Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class
        {
            using var conn = new SqlConnection(config.ConnectionString);
            await conn.OpenAsync();

            using var command = conn.CreateCommand();
            command.CommandText = config.Query;
            using var reader = await command.ExecuteReaderAsync();

            var encodingObj = Encoding.GetEncoding(config.Encoding);
            using var csvStream = new DbDataReaderCsvStream(reader, encodingObj, config.Options?.Delimiter.ToString() ?? ",");

            return await new CsvReaderService().ReadCsvAsync<T>(csvStream, config.Options, config.Encoding);
        }
    }
}
