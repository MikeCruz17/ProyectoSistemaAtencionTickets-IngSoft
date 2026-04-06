namespace ProyectoSistemaAtencionTickets.Models
{
    public class EventoSistema
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; } = DateTime.Now;
    }
}
