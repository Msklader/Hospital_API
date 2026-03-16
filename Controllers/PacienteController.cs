using Hospital_API.Dtos;
using Hospital_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        private readonly PacienteService _service;
        public PacienteController(PacienteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.Consultar_Pacientes());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _service.Consultar_PacienteXId(id);

            if (response.Exito)
                return Ok(response.Data);
            else
                return BadRequest("No se encontró al paciente solicitado");
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> InsertarPaciente([FromBody] Paciente_InsertarDto pacienteDto)
        {
            var response = await _service.Crear_Paciente(pacienteDto);
            if (!response.Exito)
                return BadRequest(response);
            else
                return Ok(response.Data);
        }

        [HttpDelete("{id_paciente}")]
        public async Task<IActionResult> EliminarPaciente(int id_paciente)
        {
            var response = await _service.Eliminar_Paciente(id_paciente);

            if (!response.Exito)
                return BadRequest(response);
            else
                return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> ActualizarPaciente([FromBody] Paciente_ActualizaDto pacienteDto)
        {
            var response = await _service.Actualizar_Paciente(pacienteDto);
            if (!response.Exito)
                return BadRequest(response);
            else
                return Ok();
        }
    }
}
