using Hospital_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Hospital_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorariosController : ControllerBase
    {
        private readonly CitasService _service;

        public HorariosController(CitasService service)
        {
            _service = service;
        }

        [HttpGet("Medico/{id_medico}/fecha/{fecha}")]
        public async Task<IActionResult> GetHorariosDisponibles(int id_medico, string fecha)
        {
            if (!DateTime.TryParseExact(fecha, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtmFecha))
            {
                return BadRequest("Formato de fecha inválido. Use dd-MM-yyyy.");
            }

            var horarios = await _service.Obtener_Horarios_Disponibles(id_medico, dtmFecha);
            return Ok(horarios);
        }
    }
}
