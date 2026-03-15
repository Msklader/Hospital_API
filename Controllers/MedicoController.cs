using Hospital_API.Dtos;
using Hospital_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicoController : ControllerBase
    {
        private readonly MedicoService _service;

        public MedicoController(MedicoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.ObtenerPacientes());
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> InsertarMedico([FromBody] Medico_CrearDto medicoDto)
        {
            return Ok(await _service.CrearMedico(medicoDto));
        }

        [HttpDelete("{id_medico}")]
        public async Task<IActionResult> EliminarrMedico(int id_medico)
        {
            var response = await _service.EliminarMedico(id_medico);

            if (!response.Exito)
                return BadRequest(response);
            else
                return Ok();


          
        }


        [HttpPut]
        public async Task<IActionResult> ActualizarMedico([FromBody] Medico_ActualizarDto medicoDto)
        {
            var response = await _service.ActualizarMedico(medicoDto);
            if (!response.Exito)
                return BadRequest(response);
            else
                return Ok();
        }
    }


}
