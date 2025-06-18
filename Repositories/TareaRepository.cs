using Dapper;
using ExamenApi.Data;
using ExamenApi.Models;
using ExamenApi.DTOs;

namespace ExamenApi.Repositories
{
    public class TareaRepository : Repository<Tarea>
    {
        public TareaRepository(DatabaseConfig dbConfig) : base(dbConfig)
        {
        }

        protected override string TableName => "Tareas";
        protected override string IdColumnName => "IdTarea";

        public override async Task<IEnumerable<Tarea>> GetAllAsync()
        {
            using var connection = _dbConfig.CreateConnection();
            var tareasDTO = await connection.QueryAsync<TareaDTO>(
                "sp_ObtenerTareas",
                commandType: System.Data.CommandType.StoredProcedure
            );

            return tareasDTO.Select(dto => new Tarea
            {
                IdTarea = dto.IdTarea,
                NombreTarea = dto.NombreTarea,
                FechaInicioPlan = dto.FechaInicioPlan,
                FechaFinPlan = dto.FechaFinPlan,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                IdRecurso = dto.IdRecurso,
                Recurso = dto.IdRecurso.HasValue ? new Empleado { IdEmpleado = dto.IdRecurso.Value, Nombre = dto.RecursoNombre } : null,
                IdPadre = dto.IdPadre,
                Predecesora = dto.Predecesora,
                Estado = dto.Estado,
                Progreso = dto.Progreso
            });
        }

        public override async Task<Tarea?> GetByIdAsync(int id)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@IdTarea", id);

            var dto = await connection.QueryFirstOrDefaultAsync<TareaDTO>(
                "sp_ObtenerTareaPorId",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (dto == null) return null;

            return new Tarea
            {
                IdTarea = dto.IdTarea,
                NombreTarea = dto.NombreTarea,
                FechaInicioPlan = dto.FechaInicioPlan,
                FechaFinPlan = dto.FechaFinPlan,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                IdRecurso = dto.IdRecurso,
                Recurso = dto.IdRecurso.HasValue ? new Empleado { IdEmpleado = dto.IdRecurso.Value, Nombre = dto.RecursoNombre } : null,
                IdPadre = dto.IdPadre,
                Predecesora = dto.Predecesora,
                Estado = dto.Estado,
                Progreso = dto.Progreso
            };
        }

        public override async Task<int> CreateAsync(Tarea entity)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new
            {
                entity.NombreTarea,
                entity.FechaInicioPlan,
                entity.FechaFinPlan,
                entity.FechaInicio,
                entity.FechaFin,
                entity.IdRecurso,
                entity.IdPadre,
                entity.Predecesora,
                entity.Estado,
                entity.Progreso
            };
            return await connection.ExecuteScalarAsync<int>(
                "sp_InsertarTarea",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
        }

        public override async Task<bool> UpdateAsync(Tarea entity)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new
            {
                entity.IdTarea,
                entity.NombreTarea,
                entity.FechaInicioPlan,
                entity.FechaFinPlan,
                entity.FechaInicio,
                entity.FechaFin,
                entity.IdRecurso,
                entity.IdPadre,
                entity.Predecesora,
                entity.Estado,
                entity.Progreso
            };
            var rowsAffected = await connection.ExecuteAsync(
                "sp_ActualizarTarea",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
            return rowsAffected > 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new { IdTarea = id };
            var rowsAffected = await connection.ExecuteAsync(
                "sp_EliminarTarea",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Tarea>> GetTareasPorEmpleado(int idEmpleado)
        {
            using var connection = _dbConfig.CreateConnection();
            var parameters = new { IdRecurso = idEmpleado };
            var tareasDTO = await connection.QueryAsync<TareaDTO>(
                "sp_ObtenerTareasPorEmpleado",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return tareasDTO.Select(dto => new Tarea
            {
                IdTarea = dto.IdTarea,
                NombreTarea = dto.NombreTarea,
                FechaInicioPlan = dto.FechaInicioPlan,
                FechaFinPlan = dto.FechaFinPlan,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                IdRecurso = dto.IdRecurso,
                Recurso = dto.IdRecurso.HasValue ? new Empleado { IdEmpleado = dto.IdRecurso.Value, Nombre = dto.RecursoNombre } : null,
                IdPadre = dto.IdPadre,
                Predecesora = dto.Predecesora,
                Estado = dto.Estado,
                Progreso = dto.Progreso
            });
        }
    }
} 