using Hospital_API.Models;
using Hospital_API.Repositories;

namespace Hospital_API.Services
{
    public class AgendaService
    {
        private readonly AgendaRepository _repo;

        public AgendaService(AgendaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Método que se encarga de consultar la agenda de un médico para una fecha específica.
        /// </summary>
        /// <param name="id_medico"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Consultar_Agenda(int id_medico, DateTime fecha)
        {
           return await _repo.Consultar_Agenda(id_medico, fecha);
        }
    }
}
