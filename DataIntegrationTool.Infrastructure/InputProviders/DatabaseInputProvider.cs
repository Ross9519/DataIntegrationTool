using System.Text;
using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Shared.Utils;
using Microsoft.Data.SqlClient;

namespace DataIntegrationTool.Infrastructure.InputProviders
{
    public class DatabaseInputProvider(ICsvReaderService csvService) : IInputProvider
    {
        private InputSourceConfig _config = default!;

        public async Task<IEnumerable<T>> CreateObjectFromInputAsync<T>() where T : class
        {
            using var conn = new SqlConnection(_config.ConnectionString);
            await conn.OpenAsync();

            using var command = conn.CreateCommand();
            command.CommandText = _config.Query;
            using var reader = await command.ExecuteReaderAsync();

            var encodingObj = Encoding.GetEncoding(_config.Encoding);
            using var csvStream = new DbDataReaderCsvStream(reader, encodingObj, _config.Options?.Delimiter.ToString() ?? ",");

            return await csvService.ReadCsvAsync<T>(csvStream, _config.Options, _config.Encoding);
        }

        public DatabaseInputProvider WithConfig(InputSourceConfig config)
        {
            _config = config;
            return this;
        }
    }
}
