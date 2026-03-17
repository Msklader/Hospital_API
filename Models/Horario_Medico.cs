namespace Hospital_API.Models
{
    public class Horario_Medico
    {
        public int id_medico { get; set; }
        public int num_dia {  get; set; }
        public TimeSpan Hora_Inicio {  get; set; }
        public TimeSpan Hora_Fin { get; set; }
    }
}
