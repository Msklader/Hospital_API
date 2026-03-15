namespace Hospital_API.Models
{
    public class Medico
    {
        public string id_medico { get; set; }
        public string nombre { get; set; }
        public string ap_paterno { get; set; }
        public string ap_materno { get; set; }
        public string status { get; set; }
        public int id_especialidad { get; set; }
        public string especialidad { get; set; }
    }
}
