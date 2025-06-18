using Dapper;
using ExamenApi.Data;
using ExamenApi.Models;
using ExamenApi.DTOs;

namespace ExamenApi.Repositories
{
    public class EmpleadoRepository : Repository<Empleado>
    {
        public EmpleadoRepository(DatabaseConfig dbConfig) : base(dbConfig)
        {
        }

        protected override string TableName => "Empleados";
        protected override string IdColumnName => "IdEmpleado";

        public override async Task<IEnumerable<Empleado>> GetAllAsync()
        {
            using var connection = _dbConfig.CreateConnection();
            var empleadosDTO = await connection.QueryAsync<EmpleadoDTO>(
                "sp_ObtenerEmpleados",
                commandType: System.Data.CommandType.StoredProcedure
            );

            return empleadosDTO.Select(dto => new Empleado
            {
                IdEmpleado = dto.IdEmpleado,
                CodigoEmpleado = dto.CodigoEmpleado,
                Nombre = dto.Nombre,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno,
                FechaNacimiento = dto.FechaNacimiento,
                FechaInicioContrato = dto.FechaInicioContrato,
                Puesto = new Puesto { NombrePuesto = dto.Puesto }
            });
        }

        public override async Task<Empleado?> GetByIdAsync(int id)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new { IdEmpleado = id };
            var dto = await connection.QueryFirstOrDefaultAsync<EmpleadoDTO>(
                "sp_ObtenerEmpleadoPorId",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (dto == null) return null;

            return new Empleado
            {
                IdEmpleado = dto.IdEmpleado,
                CodigoEmpleado = dto.CodigoEmpleado,
                Nombre = dto.Nombre,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno,
                FechaNacimiento = dto.FechaNacimiento,
                FechaInicioContrato = dto.FechaInicioContrato,
                Puesto = new Puesto { NombrePuesto = dto.Puesto }
            };
        }

        public override async Task<int> CreateAsync(Empleado entity)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new
            {
                entity.CodigoEmpleado,
                entity.Nombre,
                entity.ApellidoPaterno,
                entity.ApellidoMaterno,
                entity.FechaNacimiento,
                entity.FechaInicioContrato,
                entity.IdPuesto
            };
            return await connection.ExecuteScalarAsync<int>(
                "sp_InsertarEmpleado",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
        }

        public override async Task<bool> UpdateAsync(Empleado entity)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new
            {
                entity.IdEmpleado,
                entity.CodigoEmpleado,
                entity.Nombre,
                entity.ApellidoPaterno,
                entity.ApellidoMaterno,
                entity.FechaNacimiento,
                entity.FechaInicioContrato,
                entity.IdPuesto
            };
            var rowsAffected = await connection.ExecuteAsync(
                "sp_ActualizarEmpleado",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
            return rowsAffected > 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new { IdEmpleado = id };
            var rowsAffected = await connection.ExecuteAsync(
                "sp_EliminarEmpleado",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
            return rowsAffected > 0;
        }
    }
} 