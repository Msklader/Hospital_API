using Dapper;
using Hospital_API.Data;
using Hospital_API.Dtos;
using Hospital_API.Models;

namespace Hospital_API.Repositories
{
    public class PacienteRepository
    {
        /// <summary>
        /// Es el context, lo estoy poniendo como Data Access Object (DAO) para que sea más fácil de entender.
        /// </summary>
        private readonly AppDbContext DAO;

        public PacienteRepository(AppDbContext context)
        {
            DAO = context;
        }

        internal async Task<ResponseObj?> Consultar_Pacientes()
        {
           string query = "SELECT id_paciente, nombre, ap_paterno, ap_materno, status, fh_nacimiento, telefono, email FROM tb_cat_paciente";

            using var connection = DAO.GetConnection();
            var pacientes = await connection.QueryAsync<Paciente>(query);

            return new ResponseObj
            {
                Exito = true,
                Mensaje = "Pacientes consultados exitosamente",
                Data = pacientes
            };
        }

        /// <summary>
        /// Método encargado de consultar un paciente por su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Consultar_PacienteXId(int id)
        {
            string query = $"SELECT id_paciente, nombre, ap_paterno, ap_materno, status, fh_nacimiento, telefono, email FROM tb_cat_paciente where id_paciente = {id}";

            using var connection = DAO.GetConnection();
            var paciente = await connection.QueryFirstOrDefaultAsync<Paciente>(query);

            if(paciente == null)
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "Paciente no encontrado",
                    Data = null
                };
            }
            else
                return new ResponseObj
                {
                    Exito = true,
                    Mensaje = "Paciente consultado exitosamente",
                    Data = paciente
                };
        }

        internal async Task<ResponseObj?> Actualizar_Paciente(Paciente_ActualizaDto pacienteDto)
        {
            string query = $@" UPDATE tb_cat_paciente
                        SET nombre = '{pacienteDto.nombre}',
                            ap_paterno = '{pacienteDto.ap_paterno}',
                            ap_materno = '{pacienteDto.ap_materno}',
                            status = '{pacienteDto.status}',
                            fh_nacimiento = '{pacienteDto.fh_nacimiento:yyyy-MM-dd}',
                            telefono = '{pacienteDto.telefono}',
                            email = '{pacienteDto.email}'
                        WHERE id_paciente = {pacienteDto.id_paciente}";

            using var connection = DAO.GetConnection();
            var rowsAffected = await connection.ExecuteAsync(query);

            if (rowsAffected > 0)
            {
                return new ResponseObj
                {
                    Exito = true,
                    Mensaje = "Paciente actualizado exitosamente",
                    Data = pacienteDto
                };
            }
            else
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "Error al actualizar el paciente",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Método que se encarga de insertar un nuevo registro de paciente, datos personales
        /// </summary>
        /// <param name="pacienteDto"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Crear_Paciente(Paciente_InsertarDto pacienteDto)
        {
           string query = $@"INSERT INTO tb_cat_paciente (nombre, ap_paterno, ap_materno, status, fh_nacimiento, telefono, email)
                        VALUES ('{pacienteDto.nombre}', '{pacienteDto.ap_paterno}', '{pacienteDto.ap_materno}', '{pacienteDto.status}', '{pacienteDto.fh_nacimiento:yyyy-MM-dd}', '{pacienteDto.telefono}', '{pacienteDto.email}')


        SELECT CAST(SCOPE_IDENTITY() as int);
        ";
            using var conexion = DAO.GetConnection();
            var response = await conexion.ExecuteScalarAsync<int>(query);

            if (response > 0)
            {
                return new ResponseObj
                {
                    Exito = true,
                    Mensaje = "Paciente creado exitosamente",
                    Data = response
                };
            }
            else
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "Error al crear el paciente",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Método que se encarga de eliminar un registro de paciente
        /// </summary>
        /// <param name="id_medico"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Eliminar_Paciente(int id_medico)
        {
            string query = $"DELETE FROM tb_cat_paciente WHERE id_paciente = {id_medico}";
            using var connection = DAO.GetConnection();

            var rowsAffected = await connection.ExecuteAsync(query);
            if (rowsAffected > 0)
            {
                return new ResponseObj
                {
                    Exito = true,
                    Mensaje = "Paciente eliminado exitosamente",
                    Data = null
                };
            }
            else
            {
                return new ResponseObj
                {
                    Exito = false,
                    Mensaje = "Error al eliminar el paciente",
                    Data = null
                };
            }
        }
    }
}
