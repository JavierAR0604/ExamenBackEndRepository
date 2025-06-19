using Microsoft.AspNetCore.Mvc;
using ExamenApi.Models;
using ExamenApi.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ExamenApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PuestosController : ControllerBase
    {
        private readonly PuestoRepository _repository;

        public PuestosController(PuestoRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("ObtenerPuestos")]
        public async Task<ActionResult<IEnumerable<Puesto>>> GetPuestos()
        {
            var puestos = await _repository.GetAllAsync();
            return Ok(puestos);
        }

        [HttpPost("ObtenerPuestoPorId/{id}")]
        public async Task<ActionResult<Puesto>> GetPuesto(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El ID debe ser mayor que 0");
            }

            var puesto = await _repository.GetByIdAsync(id);
            if (puesto == null)
            {
                return NotFound($"No se encontró el puesto con ID {id}");
            }
            return Ok(puesto);
        }

        [HttpPost("CrearPuesto")]
        public async Task<ActionResult<Puesto>> CreatePuesto([FromBody] Puesto puesto)
        {
            if (string.IsNullOrWhiteSpace(puesto.NombrePuesto))
            {
                return BadRequest("El nombre del puesto es requerido");
            }

            var id = await _repository.CreateAsync(puesto);
            puesto.IdPuesto = id;
            return CreatedAtAction(nameof(GetPuesto), new { id }, puesto);
        }

        [HttpPost("ActualizarPuesto/{id}")]
        public async Task<IActionResult> UpdatePuesto(int id, [FromBody] Puesto puesto)
        {
            if (id <= 0)
            {
                return BadRequest("El ID del puesto es requerido");
            }

            if (string.IsNullOrWhiteSpace(puesto.NombrePuesto))
            {
                return BadRequest("El nombre del puesto es requerido");
            }

            puesto.IdPuesto = id;

            var success = await _repository.UpdateAsync(puesto);
            if (!success)
            {
                return NotFound($"No se encontró el puesto con ID {id}");
            }

            return NoContent();
        }

        [HttpPost("EliminarPuesto/{id}")]
        public async Task<IActionResult> DeletePuesto(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El ID debe ser mayor que 0");
            }

            var success = await _repository.DeleteAsync(id);
            if (!success)
            {
                return NotFound($"No se encontró el puesto con ID {id}");
            }

            return NoContent();
        }
    }
} 