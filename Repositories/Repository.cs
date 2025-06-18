using Dapper;
using ExamenApi.Data;
using Microsoft.Data.SqlClient;

namespace ExamenApi.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DatabaseConfig _dbConfig;
        protected abstract string TableName { get; }
        protected abstract string IdColumnName { get; }

        protected Repository(DatabaseConfig dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            using var connection = _dbConfig.CreateConnection();
            return await connection.QueryAsync<T>($"SELECT * FROM {TableName}");
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            using var connection = _dbConfig.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(
                $"SELECT * FROM {TableName} WHERE {IdColumnName} = @Id",
                new { Id = id }
            );
        }

        public virtual async Task<int> CreateAsync(T entity)
        {
            using var connection = _dbConfig.CreateConnection();
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != IdColumnName)
                .Select(p => p.Name);
            
            var columns = string.Join(", ", properties);
            var values = string.Join(", ", properties.Select(p => "@" + p));
            
            var sql = $"INSERT INTO {TableName} ({columns}) VALUES ({values}); SELECT CAST(SCOPE_IDENTITY() as int)";
            
            return await connection.QuerySingleAsync<int>(sql, entity);
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            using var connection = _dbConfig.CreateConnection();
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != IdColumnName)
                .Select(p => p.Name);
            
            var updates = string.Join(", ", properties.Select(p => $"{p} = @{p}"));
            var sql = $"UPDATE {TableName} SET {updates} WHERE {IdColumnName} = @{IdColumnName}";
            
            var rowsAffected = await connection.ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            using var connection = _dbConfig.CreateConnection();
            var sql = $"DELETE FROM {TableName} WHERE {IdColumnName} = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
} 