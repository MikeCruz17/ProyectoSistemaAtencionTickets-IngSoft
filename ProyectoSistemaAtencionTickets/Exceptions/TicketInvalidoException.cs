namespace ProyectoSistemaAtencionTickets.Exceptions
{
    public class TicketInvalidoException : Exception
    {
        public TicketInvalidoException(string mensaje) : base(mensaje)
        {
        }
    }
}
