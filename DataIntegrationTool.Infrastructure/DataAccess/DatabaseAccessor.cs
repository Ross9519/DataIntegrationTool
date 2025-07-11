using System.Data.Common;
using System.Data;
using DataIntegrationTool.Application.Factories;
using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Infrastructure.DataAccess
{
    public class DatabaseAccessor(string? connectionString) : IDatabaseAccessor
    {
        public async Task<DbDataReader> ExecuteReaderAsync(string query, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string non configurata.");

            var conn = SqlConnectionFactory.Create();
            conn.ConnectionString = connectionString;
            await conn.OpenAsync(cancellationToken);

            var command = conn.CreateCommand();
            command.CommandText = query;

            // Non facciamo using perché dobbiamo tenere aperto il reader (e la connessione) finché si legge
            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken);
        }
    }
}
