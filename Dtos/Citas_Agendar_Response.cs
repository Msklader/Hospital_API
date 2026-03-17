namespace Hospital_API.Dtos
{
    public class Citas_Agendar_Response
    {
        /// <summary>
        /// True, si la cita se agendó correctamente, false en caso contrario, por ejemplo, si el doctor no tiene horarios disponibles para la fecha y hora solicitada, o si el paciente ya tiene una cita agendada para esa fecha y hora.
        /// </summary>
        public bool Agendado { get; set; }
        /// <summary>
        /// Si todo Ok, se retornará esta variable
        /// </summary>
        public int id_cita { get; set; }
        public string Comentarios { get; set; }
        public int id_paciente { get; set; }
        public int id_medico { get; set; }
        public List<DateTime> Horarios_Disponibles { get; set; }
        /// <summary>
        /// Esta variable nos indica si el paciente tiene mas de 3 cacelaciones previas
        /// </summary>
        public bool Alerta_PacienteConCancelaciones { get; set; }

    }
}
