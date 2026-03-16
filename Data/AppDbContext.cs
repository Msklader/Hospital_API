using Hospital_API.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace Hospital_API.Data
{
    public class AppDbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection GetConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        }


        internal async Task<TransaccionWrapper> IniciarTransaccion()
        {
            try
            {
                //Iniciar transaccion
                var conexion = GetConnection();
                conexion.Open();
                var transaction = conexion.BeginTransaction();
                return new TransaccionWrapper(conexion, transaction);
            }
            catch (Exception ex)
            {
                int a = 1 + 1;
                throw;
            }
           
        }

        //Método para finalizar la transacción
        internal void FinalizarTransaccion(TransaccionWrapper transaccion, bool exito)
        {
            if (exito)
            {

                transaccion.Transaction.Commit();
            }
            else
            {
                transaccion.Transaction.Rollback();
            }
            transaccion.Conexion.Close();
        }

        internal async Task RollbackTransaccion(TransaccionWrapper transaccion)
        {
           
        }
    }
}
