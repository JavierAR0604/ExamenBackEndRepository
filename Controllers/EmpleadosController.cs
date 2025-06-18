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

        public EmpleadosController(EmpleadoRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("ObtenerEmpleados")]
        public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleados()
        {
            var empleados = await _repository.GetAllAsync();
            return Ok(empleados);
        }

        [HttpPost("ObtenerEmpleadoPorId")]
        public async Task<ActionResult<Empleado>> GetEmpleado([FromBody] int id)
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

        [HttpPost("ActualizarEmpleado")]
        public async Task<IActionResult> UpdateEmpleado([FromBody] Empleado empleado)
        {
            var success = await _repository.UpdateAsync(empleado);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("EliminarEmpleado")]
        public async Task<IActionResult> DeleteEmpleado([FromBody] int id)
        {
            var success = await _repository.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
} 