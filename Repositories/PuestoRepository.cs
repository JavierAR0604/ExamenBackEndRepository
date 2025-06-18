using Dapper;
using ExamenApi.Data;
using ExamenApi.Models;
using ExamenApi.DTOs;

namespace ExamenApi.Repositories
{
    public class PuestoRepository : Repository<Puesto>
    {
        public PuestoRepository(DatabaseConfig dbConfig) : base(dbConfig)
        {
        }

        protected override string TableName => "Puestos";
        protected override string IdColumnName => "IdPuesto";

        public override async Task<IEnumerable<Puesto>> GetAllAsync()
        {
            using var connection = _dbConfig.CreateConnection();
            var puestosDTO = await connection.QueryAsync<PuestoDTO>(
                "sp_ObtenerPuestos",
                commandType: System.Data.CommandType.StoredProcedure
            );

            return puestosDTO.Select(dto => new Puesto
            {
                IdPuesto = dto.IdPuesto,
                NombrePuesto = dto.Puesto
            });
        }

        public override async Task<Puesto?> GetByIdAsync(int id)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@IdPuesto", id);

            var dto = await connection.QueryFirstOrDefaultAsync<PuestoDTO>(
                "sp_ObtenerPuestoPorId",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (dto == null) return null;

            return new Puesto
            {
                IdPuesto = dto.IdPuesto,
                NombrePuesto = dto.Puesto
            };
        }

        public override async Task<int> CreateAsync(Puesto entity)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new { Puesto = entity.NombrePuesto };
            return await connection.ExecuteScalarAsync<int>(
                "sp_InsertarPuesto",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
        }

        public override async Task<bool> UpdateAsync(Puesto entity)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new { entity.IdPuesto, Puesto = entity.NombrePuesto };
            var rowsAffected = await connection.ExecuteAsync(
                "sp_ActualizarPuesto",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
            return rowsAffected > 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new { IdPuesto = id };
            var rowsAffected = await connection.ExecuteAsync(
                "sp_EliminarPuesto",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
            return rowsAffected > 0;
        }
    }
} 