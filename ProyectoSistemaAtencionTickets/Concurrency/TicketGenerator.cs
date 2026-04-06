using ProyectoSistemaAtencionTickets.Services;

namespace ProyectoSistemaAtencionTickets.Concurrency
{
    public class TicketGenerator
    {
        private readonly TicketMonitor _monitor;
        private readonly TicketService _ticketService;
        private bool _activo = true;
        private int _contador = 1;

        public TicketGenerator(TicketMonitor monitor, TicketService ticketService)
        {
            _monitor = monitor;
            _ticketService = ticketService;
        }

        public void Detener()
        {
            _activo = false;
        }

        public void Ejecutar()
        {
            var random = new Random();

            while (_activo)
            {
                try
                {
                    int prioridadId = random.Next(1, 4);

                    var ticket = _ticketService.CrearTicket(
                         $"Problema {_contador}",
                         $"Descripción automática del ticket {_contador}",
                         prioridadId,
                         registrarEvento: false);

                    _monitor.AgregarTicket(ticket);
                    _contador++;

                    Thread.Sleep(3000);
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
