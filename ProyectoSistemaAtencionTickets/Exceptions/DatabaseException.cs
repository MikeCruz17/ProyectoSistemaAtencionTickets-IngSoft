namespace ProyectoSistemaAtencionTickets.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string mensaje, Exception innerException)
            : base(mensaje, innerException)
        {
        }
    }
}
