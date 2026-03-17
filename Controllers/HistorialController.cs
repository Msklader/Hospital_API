using Hospital_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialController : ControllerBase
    {
        private readonly CitasService _service;

        public HistorialController(CitasService service)
        {
            _service = service;
        }


        [HttpGet("{id_paciente}")]
        public async Task<IActionResult> GetHistorialPaciente(int id_paciente)
        {
            var response = await _service.GetHistorialPaciente(id_paciente);

            if (response == null)
                return NotFound("No se encontró el paciente");
            else if (response.Exito == false)
                return NotFound(response);
            else if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}
