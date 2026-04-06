namespace ProyectoSistemaAtencionTickets.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public int EstadoId { get; set; }
        public EstadoTicket Estado { get; set; } = null!;

        public int PrioridadId { get; set; }
        public Prioridad Prioridad { get; set; } = null!;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public string? AgenteAsignado { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {Titulo} | Estado: {Estado?.Nombre ?? EstadoId.ToString()} | Prioridad: {Prioridad?.Nombre ?? PrioridadId.ToString()} | Agente: {AgenteAsignado ?? "Sin asignar"} | Fecha: {FechaCreacion}";
        }
    }
}
