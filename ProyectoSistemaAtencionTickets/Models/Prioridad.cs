namespace ProyectoSistemaAtencionTickets.Models
{
    public class Prioridad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public List<Ticket> Tickets { get; set; } = new();
    }
}
