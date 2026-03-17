namespace Hospital_API.Dtos
{
    public class AgendaDelDia_Response
    {
        public int id_cita { get; set; }
        public int id_medico { get; set; }
        public int id_paciente { get; set; }
        public DateTime fecha_cita { get; set; }
        public DateTime fecha_cita_fin { get; set; }
        public string motivo_cita { get; set; }
        public string estatus { get; set; }
        public string paciente { get; set; }
    }
}
