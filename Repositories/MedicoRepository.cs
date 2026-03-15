using Dapper;
using Hospital_API.Data;
using Hospital_API.Dtos;
using Hospital_API.Models;

namespace Hospital_API.Repositories
{
    public class MedicoRepository
    {
        /// <summary>
        /// Es el context, lo estoy poniendo como Data Access Object (DAO) para que sea más fácil de entender.
        /// </summary>
        private readonly AppDbContext DAO;

        public MedicoRepository(AppDbContext context)
        {
            DAO = context;
        }

        public async Task<List<Medico>> ObtenerTodos()
        {
            using var connection = DAO.GetConnection();

            string sql = "SELECT * FROM tb_cat_medico";

            var res = await connection.QueryAsync<Medico>(sql);
            return res.ToList();
        }

        public async Task<Medico> ObtenerPorId(int id)
        {
            using var connection = DAO.GetConnection();

            string sql = "SELECT * FROM tb_cat_medico WHERE id_medico = @Id";

            return await connection.QueryFirstOrDefaultAsync<Medico>(sql, new { Id = id });
        }    

        /// <summary>
        /// Método para insertar un nuevo médico
        /// </summary>
        /// <param name="medicoDto"></param>
        /// <returns></returns>
        internal async Task<int> InsertarMedico(Medico_CrearDto medicoDto)
        {
            using (var conexion = DAO.GetConnection())
            {
                string sql = $@"
       INSERT INTO tb_cat_medico( nombre, ap_paterno, ap_materno, status, id_especialidad) VALUES ('{medicoDto.nombre}', '{medicoDto.ap_paterno}', '{medicoDto.ap_materno}', ''{medicoDto.status}'', {medicoDto.id_especialidad});

        SELECT CAST(SCOPE_IDENTITY() as int);
        ";

                return await conexion.ExecuteScalarAsync<int>(sql);
            }    
        }

        /// <summary>
        /// Método que se encarga de eliminar un médico de la base de datos, recibe un DTO con el id del médico a eliminar y devuelve una respuesta indicando si la operación fue exitosa o no.
        /// </summary>
        /// <param name="id_medico"></param>
        /// <returns></returns>
        internal async Task<RespuestaRepository> Eliminar_Medico(int id_medico)
        {
            try
            {
                using (var conexion = DAO.GetConnection())
                {
                    string sql = $" delete from tb_cat_medico where id_medico = {id_medico}";

                    var response = await conexion.ExecuteAsync(sql);
                    if (response > 0)
                    {
                        return new RespuestaRepository { Exito = true, Mensaje = "Médico eliminado correctamente." };
                    }
                    else
                    {
                        return new RespuestaRepository { Exito = false, Mensaje = "No se pudo eliminar el médico." };
                    }
                }

            }
            catch (Exception ex)
            {
                return new RespuestaRepository { Exito = false, Mensaje = "No se pudo eliminar el médico: " + ex.Message };
            }
        }


        /// <summary>
        /// Método que se encarga de actualizar un médico en la base de datos.
        /// </summary>
        /// <param name="medicoDto"></param>
        /// <returns></returns>
        internal async Task<RespuestaRepository?> ActualizarMedico(Medico_ActualizarDto medicoDto)
        {
            string query = $@" UPDATE tb_cat_medico
                        SET nombre = '{medicoDto.nombre}',
                            ap_paterno = '{medicoDto.ap_paterno}',
                            ap_materno = '{medicoDto.ap_materno}',
                            status = '{medicoDto.status}',
                            id_especialidad = {medicoDto.id_especialidad}
                        WHERE id_medico = {medicoDto.id_medico}";
            try
            {
                using var conexion = DAO.GetConnection();
                var resultado = await conexion.ExecuteAsync(query);
                if (resultado > 0)
                {
                    return new RespuestaRepository { Exito = true, Mensaje = "Médico actualizado correctamente." };
                }
                else
                {
                    return new RespuestaRepository { Exito = false, Mensaje = "No se pudo actualizar el médico." };
                }
            }
            catch (Exception ex)
            {
                return new RespuestaRepository { Exito = false, Mensaje = "Error al actualizar el médico: " + ex.Message };
            }
        }
    }
}
