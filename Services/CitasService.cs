using Hospital_API.Dtos;
using Hospital_API.Models;
using Hospital_API.Repositories;

namespace Hospital_API.Services
{
    public class CitasService
    {
        private readonly CitasRepository _repo;

        public CitasService(CitasRepository repo)
        {
            _repo = repo;
        }


        /// <summary>
        /// Proceso para agendar una cita.
        /// </summary>
        /// <param name="agendaDto"></param>
        /// <returns></returns>
        internal async Task<Citas_Agendar_Response> Agendar_Cita(Citas_AgendarDto agendaDto)
        {
            //ERROR 409 para CONFLICTOS DE HORARIO
            string validacion = await _repo.Validar_Agenda(agendaDto);

            if (validacion != "")
            {
                var obj = new Citas_Agendar_Response
                {
                    Agendado = false,
                    Comentarios = validacion,
                    id_cita = 0,
                    id_medico = 0,
                    id_paciente = 0,
                    Horarios_Disponibles = new List<DateTime>(),
                    Alerta_PacienteConCancelaciones = false
                };

                if (validacion.Contains("el horario choca con otras citas del médico"))
                {
                    obj.Horarios_Disponibles = await Obtener_Horarios_Disponibles(agendaDto.id_medico, agendaDto.fecha_cita);
                }

                return obj;
            }

            //Si todo va bien procedemos a agendar la cita
            var response = await _repo.Agendar_Cita(agendaDto);

            Citas_Agendar_Response objRespuesta = new Citas_Agendar_Response();
            objRespuesta.Agendado = false;
            objRespuesta.Comentarios = "No se pudo agendar la cita";

            if (response.Exito)
            {
                objRespuesta.Agendado = true;
                objRespuesta.id_medico = agendaDto.id_medico;
                objRespuesta.id_paciente = agendaDto.id_paciente;
                objRespuesta.id_cita = int.Parse(response.Data.ToString());
                objRespuesta.Agendado = true;
                objRespuesta.Comentarios = "Operación realizada con éxito";
                objRespuesta.Horarios_Disponibles = new List<DateTime>();
                //Si se agendó la cita, verificamos si el paciente tiene cancelaciones previas para alertar al doctor
                objRespuesta.Alerta_PacienteConCancelaciones = await _repo.Verificar_Cancelaciones_Paciente(agendaDto.id_paciente);
            }
            return objRespuesta;

        }
        /// <summary>
        /// Método que se encargfa de cancelar una cita. Se recibe el id de la cita a cancelar y el motivo de la cancelación, se valida que la cita exista y que se pueda cancelar
        /// </summary>
        /// <param name="agendaDto"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Cancelar_Cita(Citas_CancelarDto agendaDto)
        {
            return await _repo.Cancelar_Cita(agendaDto);
        }
        /// <summary>
        /// Método que se encarga de consultar una cita por su id, se valida que la cita exista y se devuelve la información de la cita, el médico y el paciente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Consultar_Cita(int id)
        {
            return await _repo.Consultar_Cita(id);
        }


        /// <summary>
        /// Método que se encarga de obtener fechas diponibles de un médico para que el usuario pueda agendar
        /// </summary>
        /// <param name="id_medico"></param>
        /// <param name="fecha_cita"></param>
        /// <returns></returns>
        public async Task<List<DateTime>> Obtener_Horarios_Disponibles(int id_medico, DateTime fecha_cita)
        {
            //1.- Obtener los horarios del medico
            List<Horario_Medico> lstHorariosMedico = await _repo.Consultar_HorarioMedico(id_medico);
            //2.- Obtener las citas de hoy a futuro del médico
            List<CitasPaciente> lstHorariosOcupados = await _repo.Consultar_HorariosFuturosDoctor(id_medico, fecha_cita);
            //3.- Se obtiene el tiempo de consulta por tipo de medico 
            int minutosConsulta = await _repo.Consultar_Tiempo_Consulta(id_medico);

            //4.- Recomendar 5 horarios disponibles a partir de la fecha de la cita que se intentó agendar
            //4.1.- Se va ir sumando de minuto en minuto a la fecha de la cita que se intentó agendar y se va validando que ese horario no choque con los horarios ocupados del médico
            List<DateTime> lstHorariosDisponibles = new List<DateTime>();

            DateTime fechaIni = fecha_cita;
            //DateTime fechaFin;
            while (lstHorariosDisponibles.Count < 5)
            {
                //fechaFin = fechaIni.AddMinutes(minutosConsulta);

                //Validar que el horario no choque con los horarios ocupados del médico
                bool choca = lstHorariosOcupados.Any(h => fechaIni >= h.fecha_cita && fechaIni <= h.fecha_cita_fin);
                if (!choca)
                {
                    //Validar que el horario esté dentro del horario del médico
                    var horarioMedico = lstHorariosMedico.FirstOrDefault(h => h.num_dia == (int)fechaIni.DayOfWeek);
                    if (horarioMedico != null) //Si el tiempo entra dentro del horario del medico, se procede realizar la validación de horas
                    {
                        TimeSpan horaCita = fechaIni.TimeOfDay;
                        TimeSpan horaCita_Fin = fechaIni.AddMinutes(minutosConsulta).TimeOfDay;
                        if (horaCita >= horarioMedico.Hora_Inicio && horaCita_Fin <= horarioMedico.Hora_Fin)
                        {
                            lstHorariosDisponibles.Add(fechaIni.AddMinutes(-1));
                            fechaIni = fechaIni.AddMinutes(minutosConsulta - 1); //se agrega menos uno para reducir un minuto, yas que despues se sumará +1
                        }
                    }
                }

                fechaIni = fechaIni.AddMinutes(1);//Se agrega en 1 la validación para recorrer el tiempo y buscar otra posibilidad
            }

            return lstHorariosDisponibles;
        }

        #region HISTORIAL PACIENTE
        /// <summary>
        /// Método que se encarga de consultar el historial de un paciente
        /// </summary>
        /// <param name="id_paciente"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> GetHistorialPaciente(int id_paciente)
        {
            var resp = await _repo.Get_PacienteXId(id_paciente);

            if (resp == null)
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "Paciente no encontrado",
                    Data = null
                };
            }
            else if (resp.Exito)
            {
                Paciente paciente = (Paciente)resp.Data;
                var historial = await _repo.Get_Historial_Paciente(id_paciente);
                return new ResponseObj
                {
                    Exito = true,
                    Mensaje = "Historial obtenido con éxito",
                    Data = new
                    {
                        paciente.nombre,
                        paciente.ap_paterno,
                        paciente.ap_materno,
                        Historial = historial
                    }
                };
            }

            else
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "Error al obtener paciente",
                    Data = null
                };
            }
            #endregion
        }
    }
}
