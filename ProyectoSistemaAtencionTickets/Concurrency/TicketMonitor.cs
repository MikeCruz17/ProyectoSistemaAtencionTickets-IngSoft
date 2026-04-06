using ProyectoSistemaAtencionTickets.Models;

namespace ProyectoSistemaAtencionTickets.Concurrency
{
    public class TicketMonitor
    {
        private readonly Queue<Ticket> _cola = new();
        private readonly object _lock = new();

        public void AgregarTicket(Ticket ticket)
        {
            lock (_lock)
            {
                _cola.Enqueue(ticket);
                Monitor.PulseAll(_lock);
            }
        }

        public Ticket ObtenerTicket()
        {
            lock (_lock)
            {
                while (_cola.Count == 0)
                {
                    Monitor.Wait(_lock);
                }

                return _cola.Dequeue();
            }
        }

        public int CantidadPendiente()
        {
            lock (_lock)
            {
                return _cola.Count;
            }
        }
    }
}
