using Hospital_API.Dtos;
using Hospital_API.Models;
using Hospital_API.Repositories;

namespace Hospital_API.Services
{
    public class MedicoService
    {
        private readonly MedicoRepository _repo;

        public MedicoService(MedicoRepository repo)
        {
            _repo = repo;
        }
        public async Task<List<Medico>> ObtenerPacientes()
        {
            return await _repo.ObtenerTodos();
        }

        /// <summary>
        /// Método que se encarga de actualizar un registro de un médico
        /// </summary>
        /// <param name="medicoDto"></param>
        /// <returns></returns>
        internal async Task<RespuestaRepository?> ActualizarMedico(Medico_ActualizarDto medicoDto)
        {
            //1.- Verificar si existe el registro médico antes de eliminar
            var medicoExistente = await _repo.ObtenerPorId(medicoDto.id_medico);
            if (medicoExistente == null)
            {
                return new RespuestaRepository
                {
                    Exito = false,
                    Mensaje = "El médico no existe en la base de datos."
                };
            }

            return await _repo.ActualizarMedico(medicoDto);
        }

        /// <summary>
        /// Método que se encarga de crear un nuevo médico en la base de datos utilizando el repositorio de médicos.
        /// </summary>
        /// <param name="medicoDto"></param>
        /// <returns></returns>
        internal async Task<int> CrearMedico(Medico_CrearDto medicoDto)
        {
            return await _repo.InsertarMedico(medicoDto);
        }

        /// <summary>
        /// Método que se encarga de eliminar un registro de un médico
        /// </summary>
        /// <param name="id_medico"></param>
        /// <returns></returns>
        internal async Task<RespuestaRepository> EliminarMedico(int id_medico)
        {
            //1.- Verificar si existe el registro médico antes de eliminar
            var medicoExistente = await _repo.ObtenerPorId(id_medico);
            if(medicoExistente == null)
            {
                return new RespuestaRepository
                {
                    Exito = false,
                    Mensaje = "El médico no existe en la base de datos."
                };
            }

            return await _repo.Eliminar_Medico(id_medico);
        }
    }
}
