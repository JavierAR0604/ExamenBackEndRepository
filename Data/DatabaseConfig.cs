using Microsoft.Data.SqlClient;

namespace ExamenApi.Data
{
    public class DatabaseConfig
    {
        private readonly string _connectionString;

        public DatabaseConfig(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
} 