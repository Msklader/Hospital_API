namespace Hospital_API.Dtos
{
    public class Medico_ActualizarDto
    {     
        public int id_medico { get; set; }
        public string nombre { get; set; }
        public string ap_paterno { get; set; }
        public string ap_materno { get; set; }
        public string status { get; set; }
        public int id_especialidad { get; set; }
    }
}
