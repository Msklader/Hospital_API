using System.Data;

namespace Hospital_API.Models
{
    public class TransaccionWrapper
    {
        public TransaccionWrapper(IDbConnection conexion, IDbTransaction transaction)
        {
            Conexion = conexion;
            Transaction = transaction;
        }

        public IDbConnection Conexion { get; }
        public IDbTransaction Transaction { get; }
    }
}
