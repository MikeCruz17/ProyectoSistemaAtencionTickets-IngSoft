namespace ProyectoSistemaAtencionTickets.Models
{
    public class EstadoTicket
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public List<Ticket> Tickets { get; set; } = new();
    }
}
