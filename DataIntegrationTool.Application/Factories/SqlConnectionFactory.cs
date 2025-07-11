using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace DataIntegrationTool.Application.Factories
{
    public static class SqlConnectionFactory
    {
        // Factory per creare l'istanza di SqlConnection
        private static Func<SqlConnection> _create = () => new SqlConnection();

        // Metodo per ottenere una nuova SqlConnection
        public static SqlConnection Create()
        {
            return _create();
        }

        // Permette di sovrascrivere la factory per i test
        public static void SetFactory(Func<DbConnection> factory)
        {
            _create = () => (SqlConnection)factory();
        }

        // Permette di resettare la factory al comportamento di default
        public static void ResetFactory()
        {
            _create = () => new SqlConnection();
        }
    }
}
