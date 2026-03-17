namespace Hospital_API.Dtos
{
    public class Citas_AgendarDto
    {
        public int id_medico { get; set; }
        public int id_paciente { get; set; }
        public DateTime fecha_cita { get; set; }
        public string motivo_cita { get; set; }
        //public int id_tipo_cita { get; set; }
    }
}
