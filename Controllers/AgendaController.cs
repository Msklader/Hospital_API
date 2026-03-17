using Hospital_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Hospital_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgendaController : ControllerBase
    {
        private readonly AgendaService _service;
        public AgendaController(AgendaService service)
        {
            _service = service;
        }

        [HttpGet("Medico/{id_medico}/fecha/{fecha}")]
        public async Task<IActionResult> GetAgendaMedico(int id_medico, string fecha)
        {
            if (!DateTime.TryParseExact(fecha, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtmFecha))
            {
                return BadRequest("Formato de fecha inválido. Use dd-MM-yyyy.");
            }

            var response = await _service.Consultar_Agenda(id_medico, dtmFecha);

            if (response == null)
                return NotFound("No se encontró la agenda del médico solicitado");
            else if (response.Exito == false)
                return NotFound(response);
            else if (response.Exito)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}
