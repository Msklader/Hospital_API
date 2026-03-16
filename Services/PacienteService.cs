
using Hospital_API.Dtos;
using Hospital_API.Models;
using Hospital_API.Repositories;

namespace Hospital_API.Services
{
    public class PacienteService
    {
        private readonly PacienteRepository _repo;

        public PacienteService(PacienteRepository repo)
        {
            _repo = repo;
        }
        internal async Task<ResponseObj?> Consultar_Pacientes()
        {
            return await _repo.Consultar_Pacientes();
        }

        /// <summary>
        /// Método que se encarga de consultar un paciente por su identificador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Consultar_PacienteXId(int id)
        {
            return await _repo.Consultar_PacienteXId(id);
        }

        /// <summary>
        /// Método que se encargga de actualizar un registro de paciente
        /// </summary>
        /// <param name="pacienteDto"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Actualizar_Paciente(Paciente_ActualizaDto pacienteDto)
        {
            //1.- Verificar que el paciente exista
            var pacienteExistente = await _repo.Consultar_PacienteXId(pacienteDto.id_paciente);

            if (pacienteExistente == null || !pacienteExistente.Exito)
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "Paciente no encontrado",
                    Data = null
                };
            }

            return await _repo.Actualizar_Paciente(pacienteDto);
        }

        internal async Task<ResponseObj?> Crear_Paciente(Paciente_InsertarDto pacienteDto)
        {
            return await _repo.Crear_Paciente(pacienteDto);
        }
        /// <summary>
        /// Método que se encarga de eliminar un registro de paciente, primero verifica que el paciente exista, si no existe retorna un mensaje de error, caso contrario elimina el registro
        /// </summary>
        /// <param name="id_medico"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Eliminar_Paciente(int id_medico)
        {
            //1.- Verificar que el paciente exista
            var pacienteExistente = await _repo.Consultar_PacienteXId(id_medico);

            if (pacienteExistente == null || !pacienteExistente.Exito)
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "Paciente no encontrado",
                    Data = null
                };
            }

            return await _repo.Eliminar_Paciente(id_medico);
        }
    }
}
