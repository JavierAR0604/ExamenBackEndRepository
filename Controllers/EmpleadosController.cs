using Microsoft.AspNetCore.Mvc;
using ExamenApi.Models;
using ExamenApi.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ExamenApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmpleadosController : ControllerBase
    {
        private readonly EmpleadoRepository _repository;
        private readonly TareaRepository _tareaRepository;

        public EmpleadosController(EmpleadoRepository repository, TareaRepository tareaRepository)
        {
            _repository = repository;
            _tareaRepository = tareaRepository;
        }

        [HttpPost("ObtenerEmpleados")]
        public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleados()
        {
            var empleados = await _repository.GetAllAsync();
            return Ok(empleados);
        }

        [HttpPost("ObtenerEmpleadoPorId/{id}")]
        public async Task<ActionResult<Empleado>> GetEmpleado(int id)
        {
            var empleado = await _repository.GetByIdAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }
            return Ok(empleado);
        }

        [HttpPost("CrearEmpleado")]
        public async Task<ActionResult<Empleado>> CreateEmpleado([FromBody] Empleado empleado)
        {
            var id = await _repository.CreateAsync(empleado);
            empleado.IdEmpleado = id;
            return CreatedAtAction(nameof(GetEmpleado), new { id }, empleado);
        }

        [HttpPost("ActualizarEmpleado/{id}")]
        public async Task<IActionResult> UpdateEmpleado(int id, [FromBody] Empleado empleado)
        {
            empleado.IdEmpleado = id;
            var success = await _repository.UpdateAsync(empleado);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("EliminarEmpleado/{id}")]
        public async Task<IActionResult> DeleteEmpleado(int id)
        {
            // Verificar si el empleado tiene tareas asignadas
            var tareas = await _tareaRepository.GetTareasPorEmpleado(id);
            if (tareas.Any())
            {
                return BadRequest($"No se puede eliminar el empleado porque tiene {tareas.Count()} tarea(s) asignada(s).");
            }

            var success = await _repository.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return Ok(new { mensaje = "Empleado eliminado exitosamente" });
        }
    }
}

