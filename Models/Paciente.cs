namespace Hospital_API.Models
{
    public class Paciente
    {
        public int id_paciente { get; set; }
        public string nombre { get; set; }
        public string ap_paterno { get; set; }
        public string ap_materno { get; set; }
        public string status { get; set; }
        public DateTime fh_nacimiento { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
    }
}
