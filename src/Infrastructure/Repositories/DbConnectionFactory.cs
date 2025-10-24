using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories
{
    public interface IDbConnectionFactory
    {
        public IDbConnection CreateConnection();
        public Task<IDbConnection> CreateConnectionAsync();
    }

    public class SqlServerConnectionFactory(string connectionString) : IDbConnectionFactory
    {
        public IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}