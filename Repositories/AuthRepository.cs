using Dapper;
using ExamenApi.Data;
using ExamenApi.Models;

namespace ExamenApi.Repositories
{
    public class AuthRepository
    {
        private readonly DatabaseConfig _dbConfig;

        public AuthRepository(DatabaseConfig dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public async Task<Usuario?> ValidarUsuario(string usuario, string contrasena)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new { Usuario = usuario, Contrasena = contrasena };
            
            return await connection.QueryFirstOrDefaultAsync<Usuario>(
                "sp_ValidarUsuario",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
        }
    }
} 