using Azure;
using Hospital_API.Dtos;
using Hospital_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {
        private readonly CitasService _service;
        public CitasController(CitasService service)
        {
            _service = service;
        }

        [HttpPost("Agendar")]
        public async Task<IActionResult> AgendarCita([FromBody] Citas_AgendarDto agendaDto)
        {
            try
            {
                var response = await _service.Agendar_Cita(agendaDto);

                if(response == null)
                    return BadRequest("No se pudo agendar la cita. Verifique los datos e intente nuevamente.");
                else if (response.Agendado)
                    return Ok(response); 
                else if(response.Agendado == false && response.Comentarios != "No se pudo agendar la cita")
                    return Conflict(response);
                else
                    return BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }          
        }

        [HttpPut("Cancelar")]
        public async Task<IActionResult> CancelarCita([FromBody] Citas_CancelarDto agendaDto)
        {
            try
            {
                var response = await _service.Cancelar_Cita(agendaDto);

                if (response == null || response.Exito == false)
                    return BadRequest(response);
                else
                {
                    return Ok(response);
                }                   
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Consultar/{id}")]
        public async Task<IActionResult> ConsultarCita(int id)
        {
            try
            {
                var response = await _service.Consultar_Cita(id);

                if (response == null )
                    return BadRequest(response);
                else if (response.Exito == false)
                    return NotFound(response);
                else if(response.Exito == true)
                    return Ok(response);
                else 
                    return BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
