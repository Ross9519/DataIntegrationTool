using System.Data.Common;

namespace DataIntegrationTool.Application.Interfaces
{
    public interface IDatabaseAccessor
    {
        Task<DbDataReader> ExecuteReaderAsync(string query, CancellationToken cancellationToken = default);
    }
}
