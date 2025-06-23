using Microsoft.AspNetCore.Mvc;
using ExamenApi.Models;
using ExamenApi.Repositories;
using ExamenApi.DTOs;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace ExamenApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TareasController : ControllerBase
    {
        private readonly TareaRepository _tareaRepository;

        public TareasController(TareaRepository tareaRepository)
        {
            _tareaRepository = tareaRepository;
        }

        [HttpPost("ObtenerTareas")]
        public async Task<ActionResult<IEnumerable<TareaDTO>>> GetAll()
        {
            try
            {
                var tareas = await _tareaRepository.GetAllAsync();
                var tareasDTO = tareas.Select(t => new TareaDTO
                {
                    IdTarea = t.IdTarea,
                    NombreTarea = t.NombreTarea,
                    FechaInicioPlan = t.FechaInicioPlan,
                    FechaFinPlan = t.FechaFinPlan,
                    FechaInicio = t.FechaInicio,
                    FechaFin = t.FechaFin,
                    IdRecurso = t.IdRecurso,
                    RecursoNombre = t.Recurso?.Nombre ?? string.Empty,
                    IdPadre = t.IdPadre,
                    Predecesora = t.Predecesora,
                    Estado = t.Estado,
                    Progreso = t.Progreso
                });
                return Ok(tareasDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("ObtenerTareaPorId/{id}")]
        public async Task<ActionResult<TareaDTO>> GetById( int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("El ID de la tarea debe ser mayor que 0");
                }

                var tarea = await _tareaRepository.GetByIdAsync(id);
                if (tarea == null)
                {
                    return NotFound($"No se encontró la tarea con ID {id}");
                }

                var tareaDTO = new TareaDTO
                {
                    IdTarea = tarea.IdTarea,
                    NombreTarea = tarea.NombreTarea,
                    FechaInicioPlan = tarea.FechaInicioPlan,
                    FechaFinPlan = tarea.FechaFinPlan,
                    FechaInicio = tarea.FechaInicio,
                    FechaFin = tarea.FechaFin,
                    IdRecurso = tarea.IdRecurso,
                    RecursoNombre = tarea.Recurso?.Nombre ?? string.Empty,
                    IdPadre = tarea.IdPadre,
                    Predecesora = tarea.Predecesora,
                    Estado = tarea.Estado,
                    Progreso = tarea.Progreso
                };

                return Ok(tareaDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("CrearTarea")]
        public async Task<ActionResult<TareaDTO>> Create([FromBody] TareaDTO tareaDTO)
        {
            try
            {
                if (tareaDTO == null)
                {
                    return BadRequest("Los datos de la tarea son requeridos");
                }

                if (string.IsNullOrWhiteSpace(tareaDTO.NombreTarea))
                {
                    return BadRequest("El nombre de la tarea es requerido");
                }

                tareaDTO.Estado = GetEstadoFromProgreso(tareaDTO.Progreso);

                var tarea = new Tarea
                {
                    NombreTarea = tareaDTO.NombreTarea,
                    FechaInicioPlan = tareaDTO.FechaInicioPlan,
                    FechaFinPlan = tareaDTO.FechaFinPlan,
                    FechaInicio = tareaDTO.FechaInicio,
                    FechaFin = tareaDTO.FechaFin,
                    IdRecurso = tareaDTO.IdRecurso,
                    IdPadre = tareaDTO.IdPadre,
                    Predecesora = tareaDTO.Predecesora,
                    Estado = tareaDTO.Estado,
                    Progreso = tareaDTO.Progreso
                };

                var id = await _tareaRepository.CreateAsync(tarea);
                tareaDTO.IdTarea = id;

                return Ok(tareaDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("ActualizarTarea/{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] TareaDTO tareaDTO)
        {
            try
            {
                if (tareaDTO == null)
                {
                    return BadRequest("Los datos de la tarea son requeridos");
                }

                if (id <= 0)
                {
                    return BadRequest("El ID de la tarea debe ser mayor que 0");
                }

                var tareaExistente = await _tareaRepository.GetByIdAsync(id);
                if (tareaExistente == null)
                {
                    return NotFound($"No se encontró la tarea con ID {id}");
                }

                tareaDTO.IdTarea = id;

                tareaDTO.Estado = GetEstadoFromProgreso(tareaDTO.Progreso);

                var tarea = new Tarea
                {
                    IdTarea = tareaDTO.IdTarea,
                    NombreTarea = tareaDTO.NombreTarea,
                    FechaInicioPlan = tareaDTO.FechaInicioPlan,
                    FechaFinPlan = tareaDTO.FechaFinPlan,
                    FechaInicio = tareaDTO.FechaInicio,
                    FechaFin = tareaDTO.FechaFin,
                    IdRecurso = tareaDTO.IdRecurso,
                    IdPadre = tareaDTO.IdPadre,
                    Predecesora = tareaDTO.Predecesora,
                    Estado = tareaDTO.Estado,
                    Progreso = tareaDTO.Progreso
                };

                var success = await _tareaRepository.UpdateAsync(tarea);
                if (!success)
                {
                    return StatusCode(500, "Error al actualizar la tarea");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Tarea recibida para actualizar:");
                Console.WriteLine(JsonSerializer.Serialize(tareaDTO));
                Console.WriteLine("Error al actualizar tarea:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private string GetEstadoFromProgreso(int? progreso)
        {
            if (progreso == null || progreso == 0) return "Asignado";
            if (progreso > 0 && progreso < 100) return "En progreso";
            if (progreso == 100) return "Completada";
            return string.Empty;
        }

        [HttpPost("eliminar/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("El ID de la tarea debe ser mayor que 0");
                }

                var tareaExistente = await _tareaRepository.GetByIdAsync(id);
                if (tareaExistente == null)
                {
                    return NotFound($"No se encontró la tarea con ID {id}");
                }

                var success = await _tareaRepository.DeleteAsync(id);
                if (!success)
                {
                    return StatusCode(500, "Error al eliminar la tarea");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("por-empleado/{idEmpleado}")]
        public async Task<ActionResult<IEnumerable<TareaDTO>>> GetTareasPorEmpleado(int idEmpleado)
        {
            try
            {
                if (idEmpleado <= 0)
                {
                    return BadRequest("El ID del empleado debe ser mayor que 0");
                }

                var tareas = await _tareaRepository.GetTareasPorEmpleado(idEmpleado);
                var tareasDTO = tareas.Select(t => new TareaDTO
                {
                    IdTarea = t.IdTarea,
                    NombreTarea = t.NombreTarea,
                    FechaInicioPlan = t.FechaInicioPlan,
                    FechaFinPlan = t.FechaFinPlan,
                    FechaInicio = t.FechaInicio,
                    FechaFin = t.FechaFin,
                    IdRecurso = t.IdRecurso,
                    RecursoNombre = t.Recurso?.Nombre ?? string.Empty,
                    IdPadre = t.IdPadre,
                    Predecesora = t.Predecesora,
                    Estado = t.Estado,
                    Progreso = t.Progreso
                });
                return Ok(tareasDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
} 

 

