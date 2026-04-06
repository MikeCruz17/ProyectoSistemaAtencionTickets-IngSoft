using ProyectoSistemaAtencionTickets.Services;

namespace ProyectoSistemaAtencionTickets.Concurrency
{
    public class AgentWorker
    {
        private readonly string _nombre;
        private readonly TicketMonitor _monitor;
        private readonly TicketService _ticketService;

        public AgentWorker(string nombre, TicketMonitor monitor, TicketService ticketService)
        {
            _nombre = nombre;
            _monitor = monitor;
            _ticketService = ticketService;
        }

        public void Ejecutar()
        {
            var random = new Random();

            while (true)
            {
                try
                {
                    var ticket = _monitor.ObtenerTicket();

                    _ticketService.MarcarEnProceso(ticket.Id, _nombre);

                    Thread.Sleep(random.Next(2000, 5000));

                    _ticketService.CerrarTicket(ticket.Id, _nombre);
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
                catch (Exception)
                {
                    // Error omitido en consola para no interferir con el menú.
                }
            }
        }
    }
}
