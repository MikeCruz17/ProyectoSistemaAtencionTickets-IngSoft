namespace ProyectoSistemaAtencionTickets.Models
{
    public class Agente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Disponible { get; set; } = true;
    }
}
