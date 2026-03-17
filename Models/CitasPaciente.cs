namespace Hospital_API.Models
{
    public class CitasPaciente
    {
        public int id_cita { get; set; }
        public int id_medico { get; set; }
        public DateTime fecha_cita { get; set; }
        public DateTime fecha_cita_fin { get; set; }
        public string motivo_cita { get; set; }
        public int id_tipo_cita { get; set; }
        public string? motivo_cancelacion { get; set; }
        public string? status { get; set; }
    }
}
