using Dapper;
using Hospital_API.Data;
using Hospital_API.Dtos;
using Hospital_API.Models;

namespace Hospital_API.Repositories
{
    public class AgendaRepository
    {
        /// <summary>
        /// Es el context, lo estoy poniendo como Data Access Object (DAO) para que sea más fácil de entender.
        /// </summary>
        private readonly AppDbContext DAO;

        public AgendaRepository(AppDbContext context)
        {
            DAO = context;
        }

        /// <summary>
        /// Método que se encarga de consultar una agenda de un medico para un día en especifico
        /// </summary>
        /// <param name="id_medico"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Consultar_Agenda_anterior(int id_medico, DateTime fecha)
        {     
              string query = $@"SELECT id_cita, id_medico, id_paciente, fecha_cita, fecha_cita_fin, motivo_cita, id_tipo_cita, motivo_cancelacion, status
                              FROM tb_citas_paciente
                              WHERE id_medico = {id_medico} AND CAST(fecha_cita AS DATE) = '{fecha.ToString("yyyy-MM-dd")}'
                                and status = 'A' 
                                order by fecha_cita";

            using var conexion = DAO.GetConnection();

            var results = await conexion.QueryAsync<CitasPaciente>(query);

            return new ResponseObj
            {
                Exito = true,
                Mensaje = "Consulta exitosa",
                Data = results.ToList()
            };

        }

        /// <summary>
        /// Método que se encarga de consultar una agenda de un medico para un día en especifico
        /// </summary>
        /// <param name="id_medico"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Consultar_Agenda(int id_medico, DateTime fecha)
        {
            string query = $@"exec sp_agenda_consultar {id_medico}, '{fecha.ToString("yyyy-MM-dd")}'";

            using var conexion = DAO.GetConnection();

            var results = await conexion.QueryAsync<AgendaDelDia_Response>(query);

            return new ResponseObj
            {
                Exito = true,
                Mensaje = "Consulta exitosa",
                Data = results.ToList()
            };

        }
    }
}
