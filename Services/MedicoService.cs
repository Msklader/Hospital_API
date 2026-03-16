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

        public async Task<List<Medico>> Consultar_Medicos()
        {
            return await _repo.Consultar_Todos();
        }
        internal async Task<ResponseObj?> Consultar_MedicoXId(int id)
        {
            return await _repo.Consultar_MedicoXId(id);
        }
        /// <summary>
        /// Método que se encarga de crear un nuevo médico en la base de datos utilizando el repositorio de médicos.
        /// </summary>
        /// <param name="medicoDto"></param>
        /// <returns></returns>
        internal async Task<ResponseObj> CrearMedico(Medico_CrearDto medicoDto)
        {
            return await _repo.InsertarMedico(medicoDto);
        }

        /// <summary>
        /// Método que se encarga de actualizar un registro de un médico
        /// </summary>
        /// <param name="medicoDto"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> ActualizarMedico(Medico_ActualizarDto medicoDto)
        {
            //1.- Verificar si existe el registro médico antes de eliminar
            var medicoExistente = await _repo.ObtenerPorId(medicoDto.id_medico);
            if (medicoExistente == null)
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "El médico no existe en la base de datos."
                };
            }

            return await _repo.ActualizarMedico(medicoDto);
        }

        /// <summary>
        /// Método que se encarga de eliminar un registro de un médico
        /// </summary>
        /// <param name="id_medico"></param>
        /// <returns></returns>
        internal async Task<ResponseObj> EliminarMedico(int id_medico)
        {
            //1.- Verificar si existe el registro médico antes de eliminar
            var medicoExistente = await _repo.ObtenerPorId(id_medico);
            if (medicoExistente == null)
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "El médico no existe en la base de datos."
                };
            }

            return await _repo.Eliminar_Medico(id_medico);
        }
        /// <summary>
        /// Método que se encarga de asignar los horarios a un médico, primero elimina los horarios anteriores del médico y luego inserta los nuevos horarios, todo esto dentro de una transacción para evitar que queden datos inconsistentes en la base de datos en caso de que ocurra un error durante el proceso.
        /// </summary>
        /// <param name="id_medico"></param>
        /// <param name="horarioDto"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> AsignarHorario(int id_medico, List<Medico_HorariosDto> horarioDto)
        {
            //Crear una transacción, ya que se van a eliminar los horarios anteriores y se van a insertar los nuevos horarios, si ocurre un error en el proceso, se debe hacer un rollback de la transacción para evitar que queden datos inconsistentes en la base de datos.
            await _repo.CrearTransaccion();
            {
                try
                {
                    //1.- Verificar si existe el registro médico antes de eliminar
                    var medicoExistente = await _repo.ObtenerPorId(id_medico);
                    if (medicoExistente == null)
                    {
                        return new ResponseObj
                        {
                            Exito = false,
                            Mensaje = "El médico no existe en la base de datos."
                        };
                    }

                    //2.- Eliminar los horarios anteriores del médico
                    await _repo.Eliminar_Horarios_Medico(id_medico);

                    //3.- Insertar los nuevos horarios del médico
                    foreach (var horario in horarioDto)
                    {
                        await _repo.Insertar_Horarios_Medico(id_medico, horario);
                    }

                    //4.- Commit de la transacción
                    await _repo.CommitTransaccion();
                    return new ResponseObj
                    {
                        Exito = true,
                        Mensaje = "Los horarios del médico se han asignado correctamente."
                    };
                }
                catch (Exception ex)
                {
                    await _repo.RollbackTransaccion();
                    return new ResponseObj
                    {
                        Exito = false,
                        Mensaje = $"Ocurrió un error al eliminar los horarios anteriores del médico: {ex.Message}"
                    };
                }
            }
        }

      
    }
}
