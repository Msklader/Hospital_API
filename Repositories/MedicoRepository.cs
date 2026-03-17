using Dapper;
using Hospital_API.Data;
using Hospital_API.Dtos;
using Hospital_API.Models;
using System.Data;
using System.Transactions;

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

        public async Task<List<Medico>> Consultar_Todos()
        {
            using var connection = DAO.GetConnection();

            string sql = "SELECT * FROM tb_cat_medico";

            var res = await connection.QueryAsync<Medico>(sql);
            return res.ToList();
        }

        /// <summary>
        /// Método que se encarga de consultar un medico por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> Consultar_MedicoXId(int id)
        {
            using var connection = DAO.GetConnection();
            string sql = $"SELECT * FROM tb_cat_medico where id_medico = {id}";
            var res = await connection.QueryFirstOrDefaultAsync<Medico>(sql);

            if (res == null)            
                return new ResponseObj { Exito = false, Mensaje = "No se encontró el médico con el ID proporcionado." };            
            else            
                return new ResponseObj { Exito = true, Mensaje = "Médico encontrado.", Data = res };            
           
        }

        public async Task<Medico> ObtenerPorId(int id)
        {
            //Verificar si hay alguna transaccion en proceso, si es así, usar la misma conexión y transacción para garantizar la atomicidad de las operaciones
            if (objTransaccion != null)
            {
                return await objTransaccion.Conexion.QueryFirstOrDefaultAsync<Medico>("SELECT * FROM tb_cat_medico WHERE id_medico = @Id", new { Id = id }, transaction: objTransaccion.Transaction);
            }
            else
            {
                using var connection = DAO.GetConnection();
                string sql = "SELECT * FROM tb_cat_medico WHERE id_medico = @Id";

                return await connection.QueryFirstOrDefaultAsync<Medico>(sql, new { Id = id });
            }
        }    

        /// <summary>
        /// Método para insertar un nuevo médico
        /// </summary>
        /// <param name="medicoDto"></param>
        /// <returns></returns>
        public async Task<ResponseObj> InsertarMedico(Medico_CrearDto medicoDto)
        {
            using (var conexion = DAO.GetConnection())
            {
                string sql = $@"
       INSERT INTO tb_cat_medico( nombre, ap_paterno, ap_materno, status, id_especialidad) VALUES ('{medicoDto.nombre}', '{medicoDto.ap_paterno}', '{medicoDto.ap_materno}', '{medicoDto.status}', {medicoDto.id_especialidad});

        SELECT CAST(SCOPE_IDENTITY() as int);
        ";

                var id = await conexion.ExecuteScalarAsync<int>(sql);
                if(id > 0)
                    return new ResponseObj { Exito = true, Mensaje = "Médico insertado correctamente.", Data = id };
                else 
                    return new ResponseObj { Exito = false, Mensaje = "No se pudo insertar el médico." };
            }    
        }

        /// <summary>
        /// Método que se encarga de eliminar un médico de la base de datos, recibe un DTO con el id del médico a eliminar y devuelve una respuesta indicando si la operación fue exitosa o no.
        /// </summary>
        /// <param name="id_medico"></param>
        /// <returns></returns>
        internal async Task<ResponseObj> Eliminar_Medico(int id_medico)
        {
            try
            {
                using (var conexion = DAO.GetConnection())
                {
                    string sql = $" delete from tb_cat_medico where id_medico = {id_medico}";

                    var response = await conexion.ExecuteAsync(sql);
                    if (response > 0)
                    {
                        return new ResponseObj { Exito = true, Mensaje = "Médico eliminado correctamente." };
                    }
                    else
                    {
                        return new ResponseObj { Exito = false, Mensaje = "No se pudo eliminar el médico." };
                    }
                }

            }
            catch (Exception ex)
            {
                return new ResponseObj { Exito = false, Mensaje = "No se pudo eliminar el médico: " + ex.Message };
            }
        }


        /// <summary>
        /// Método que se encarga de actualizar un médico en la base de datos.
        /// </summary>
        /// <param name="medicoDto"></param>
        /// <returns></returns>
        internal async Task<ResponseObj?> ActualizarMedico(Medico_ActualizarDto medicoDto)
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
                    return new ResponseObj { Exito = true, Mensaje = "Médico actualizado correctamente." };
                }
                else
                {
                    return new ResponseObj { Exito = false, Mensaje = "No se pudo actualizar el médico." };
                }
            }
            catch (Exception ex)
            {
                return new ResponseObj { Exito = false, Mensaje = "Error al actualizar el médico: " + ex.Message };
            }
        }


        #region TRANSACCIONES
        //Se obtiene la transacción y la conexión de la base de datos para realizar operaciones atómicas, es decir, que si una operación falla, se puedan revertir todas las operaciones

        TransaccionWrapper objTransaccion = null;
        internal async Task CrearTransaccion()
        {
            objTransaccion = await DAO.IniciarTransaccion();
        }

        internal async Task CommitTransaccion()
        {
            if (objTransaccion != null)
            {
                objTransaccion.Transaction.Commit();
                objTransaccion.Conexion.Close();
            }
        }

        internal async Task RollbackTransaccion()
        {
            if (objTransaccion != null)
            {
                objTransaccion.Transaction.Rollback();
                objTransaccion.Conexion.Close();
            }
        }
        #endregion

        internal async Task Eliminar_Horarios_Medico(int id_medico)
        {
           string query = $"DELETE FROM tb_horario_medico WHERE id_medico = {id_medico}";
            await objTransaccion.Conexion.ExecuteAsync(query, transaction: objTransaccion.Transaction);
        }


        internal async Task Insertar_Horarios_Medico(int id_medico, Medico_HorariosDto horario)
        {
            //El formato de la hora debe ser 'HH:mm:ss' para que se inserte correctamente en la base de datos, si no se hace así, se insertará la hora con el formato 'yyyy-MM-dd HH:mm:ss' y no se podrá consultar correctamente, es una columna de tipo Time
           // string hora_inicio = horario.Hora_Inicio.ToString("HH:mm:ss");
           // string hora_fin = horario.Hora_Fin.ToString("HH:mm:ss");
            string query = $"INSERT INTO tb_horario_medico(id_medico, num_dia, hora_inicio, hora_fin) VALUES ({id_medico}, {horario.Num_Dia}, '{horario.Hora_Inicio}', '{horario.Hora_Fin}')";
            await objTransaccion.Conexion.ExecuteAsync(query, transaction: objTransaccion.Transaction);
        }

      
    }
}
