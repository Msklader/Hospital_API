using Dapper;
using Hospital_API.Data;
using Hospital_API.Dtos;
using Hospital_API.Models;
using Microsoft.Data.SqlClient;

namespace Hospital_API.Repositories
{
    public class CitasRepository
    {
        /// <summary>
        /// Es el context, lo estoy poniendo como Data Access Object (DAO) para que sea más fácil de entender.
        /// </summary>
        private readonly AppDbContext DAO;

        public CitasRepository(AppDbContext context)
        {
            DAO = context;
        }

        /// <summary>
        /// Método que se encarga de agendar una cita
        /// </summary>
        /// <param name="agendaDto"></param>
        /// <returns></returns>
        internal async Task<ResponseObj> Agendar_Cita(Citas_AgendarDto agendaDto)
        {
           string query = $@"exec sp_citas_agendar
	                            {agendaDto.id_medico},
	                            {agendaDto.id_paciente},
	                            '{agendaDto.fecha_cita.ToString("yyyy-MM-dd HH:mm:ss")}',
	                            '{agendaDto.motivo_cita}',
	                            1";

            using var conexion = DAO.GetConnection();
            
            var idCita = await conexion.QuerySingleAsync<int>(query);

            if(idCita <= 0)
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "No se pudo agendar la cita",
                    Data = null
                };
            }
            else 
                return new ResponseObj
                {
                    Exito = true,
                    Mensaje = "Cita agendada exitosamente",
                    Data = idCita
                };
        }

        /// <summary>
        /// Método que se encarga de cancelar una cita de hospital. El SP tiene una validación para que no se puedan cancelar citas pasadas.
        /// </summary>
        /// <param name="agendaDto"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Cancelar_Cita(Citas_CancelarDto agendaDto)
        {
            try
            {
                string query = $@"exec sp_citas_cancelar
                                {agendaDto.id_cita},
                                '{agendaDto.motivo_cancelacion}'";
                using var conexion = DAO.GetConnection();
                await conexion.ExecuteAsync(query);
                return new ResponseObj
                {
                    Exito = true,
                    Mensaje = "Cita cancelada exitosamente",
                    Data = null
                };
            }
            catch (SqlException ex)
            {
                if (ex.Number == 51000) //Codigo de error personalizado generado en el sp
                {
                    return new ResponseObj
                    {
                        Exito = false,
                        Mensaje = "Validación: " + ex.Message,
                        Data = null
                    };
                }
                else
                    return new ResponseObj
                    {
                        Exito = false,
                        Mensaje = "Error: " + ex.Message,
                        Data = null
                    };
            }
            catch (Exception ex)
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "Error al cancelar la cita: " + ex.Message,
                    Data = null
                };
            }
        }

        /// <summary>
        /// Método que se encarga de consultar una cita por su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Consultar_Cita(int id)
        {
            string query = $@"SELECT id_cita, id_medico, id_paciente, fecha_cita, fecha_cita_fin, motivo_cita, id_tipo_cita, motivo_cancelacion, status from tb_citas_paciente where id_cita = {id}";
            using var conexion = DAO.GetConnection();
            var cita = await conexion.QuerySingleOrDefaultAsync<CitasPaciente>(query);
            if (cita == null)
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "Cita no encontrada",
                    Data = null
                };
            }
            return new ResponseObj
            {
                Exito = true,
                Mensaje = "Cita encontrada",
                Data = cita
            };
        }

        /// <summary>
        /// Método que se encarga de consultar los horarios de médico
        /// </summary>
        /// <param name="id_medico"></param>
        /// <returns></returns>
        internal async Task<List<Horario_Medico>> Consultar_HorarioMedico(int id_medico)
        {
            string query = $"SELECT id_medico, num_dia, hora_inicio, hora_fin FROM tb_horario_medico where id_medico = {id_medico} ";
            using var conexion = DAO.GetConnection();
            var horarios = await conexion.QueryAsync<Horario_Medico>(query);
            return horarios.ToList();
        }

        /// <summary>
        /// Método para obtener 5 horarios disponibles de un médico en una fecha determinada o superior.
        /// </summary>
        /// <param name="id_medico"></param>
        /// <param name="fecha_cita"></param>
        /// <returns></returns>
        internal async Task<List<CitasPaciente>> Consultar_HorariosFuturosDoctor(int id_medico, DateTime fecha_cita)
        {
            string query = $@"SELECT id_cita, id_medico, id_paciente, fecha_cita, fecha_cita_fin, motivo_cita, id_tipo_cita, motivo_cancelacion, status 
FROM tb_citas_paciente
where id_medico = {id_medico}
and fecha_cita >= '{fecha_cita.ToString("yyyy-MM-dd HH:mm:ss")}'
order by fecha_cita";

            using var conexion = DAO.GetConnection();
            var horarios = await conexion.QueryAsync<CitasPaciente>(query);
            return horarios.ToList();
        }
        /// <summary>
        /// Se obtiene el tiempo de consulta de un medico por su id
        /// </summary>
        /// <param name="id_medico"></param>
        /// <returns></returns>
        internal async Task<int> Consultar_Tiempo_Consulta(int id_medico)
        {
            string query = $"SELECT duracion_consulta FROM tb_cat_especialidades where id_especialidad = (select id_especialidad from tb_cat_medico where id_medico = {id_medico}) ";
            using var conexion = DAO.GetConnection();
            var tiempoConsulta = await conexion.QuerySingleAsync<int>(query);
            return tiempoConsulta;
        }

        

        /// <summary>
        /// Se ejecuta un sp el cual realiza validaciones, los detalles son retornados por SQL 
        /// </summary>
        /// <param name="agendaDto"></param>
        /// <returns></returns>
        internal async Task<string> Validar_Agenda(Citas_AgendarDto agendaDto)
        {
            try
            {
                //1.- Se manda a ejecutar un sp para que realice las validaciones necesarias para agendar la cita.
                string query = $"EXEC sp_citas_validar_nuevo {agendaDto.id_medico}, {agendaDto.id_paciente}, '{agendaDto.fecha_cita.ToString("yyyy-MM-dd HH:mm:ss")}'";

                using var conexion = DAO.GetConnection();
                await conexion.ExecuteAsync(query);
                return "";
            }
            catch (SqlException ex)
            {                
                if(ex.Number == 51000) //Codigo de error personalizado generado en el sp
                {
                    return "Validación: " + ex.Message; 
                }
                else
                    return "Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;                
            }
        }

        /// <summary>
        /// Método que se encarga de verificar si un paciente tiene más de 3 cancelaciones en los últimos 30 días, para alertar al momento de agendar la cita
        /// </summary>
        /// <param name="id_paciente"></param>
        /// <returns></returns>
        internal async Task<bool> Verificar_Cancelaciones_Paciente(int id_paciente)
        {
            string query = $@"SELECT isnull(COUNT(*), 0) 
                                FROM tb_citas_paciente 
                                WHERE id_paciente = {id_paciente} 
                                AND status = 'CN'
                                and fecha_cita >= DATEADD(day, -30, GETDATE())
                                and fecha_cita <= GETDATE()";

            using var conexion = DAO.GetConnection();
            int cancelaciones = await conexion.QuerySingleAsync<int>(query);
            return cancelaciones > 3;
        }


        #region HISTORIAL PACIENTE
        internal async Task<ResponseObj?> GetHistorialPaciente(int id_paciente)
        {
            string query = $@"SELECT id_cita, id_medico, id_paciente, fecha_cita, fecha_cita_fin, motivo_cita, id_tipo_cita, motivo_cancelacion, status
                              FROM tb_citas_paciente
                              WHERE id_paciente = {id_paciente}
                              ORDER BY fecha_cita DESC";

            using var conexion = DAO.GetConnection();
            var historial = await conexion.QueryAsync<CitasPaciente>(query);
            return new ResponseObj
            {
                Exito = true,
                Data = historial.ToList()
            };
        }

        /// <summary>
        /// Método que se encarga de consultar un paciente por id 
        /// </summary>
        /// <param name="id_paciente"></param>
        /// <returns></returns>
        internal async Task<ResponseObj> Get_PacienteXId(int id_paciente)
        {
            PacienteRepository repoPacientes = new PacienteRepository(DAO);
            var response = await repoPacientes.Consultar_PacienteXId(id_paciente);
            return response;
        }
        /// <summary>
        /// Método que se encarga dde consultar el historial del paciente
        /// </summary>
        /// <param name="id_paciente"></param>
        /// <returns></returns>
        internal async Task<List<CitasPaciente>> Get_Historial_Paciente(int id_paciente)
        {
           string query = $@"SELECT id_cita, id_medico, id_paciente, fecha_cita, fecha_cita_fin, motivo_cita, id_tipo_cita, motivo_cancelacion, status
                              FROM tb_citas_paciente
                              WHERE id_paciente = {id_paciente}
                              ORDER BY fecha_cita DESC";
            using var conexion = DAO.GetConnection();
            var historial = await conexion.QueryAsync<CitasPaciente>(query);
            return historial.ToList();
        }
        #endregion
    }
}
