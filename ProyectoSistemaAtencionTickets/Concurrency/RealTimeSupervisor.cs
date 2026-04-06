using ProyectoSistemaAtencionTickets.Services;

namespace ProyectoSistemaAtencionTickets.Concurrency
{
    public class RealtimeSupervisor
    {
        private readonly TicketMonitor _monitor;
        private readonly TicketService _ticketService;
        private bool _activo = true;

        private bool _saturacionActiva = false;
        private DateTime _ultimaAlerta = DateTime.MinValue;
        private readonly TimeSpan _intervaloMinimoAlerta = TimeSpan.FromSeconds(30);

        public RealtimeSupervisor(TicketMonitor monitor, TicketService ticketService)
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
            while (_activo)
            {
                try
                {
                    int pendientes = _monitor.CantidadPendiente();
                    bool haySaturacion = pendientes >= 5;

                    if (haySaturacion)
                    {
                        bool debeRegistrar =
                            !_saturacionActiva ||
                            DateTime.Now - _ultimaAlerta >= _intervaloMinimoAlerta;

                        if (debeRegistrar)
                        {
                            _ticketService.RegistrarEvento(
                                "ALERTA",
                                $"Saturación del sistema: {pendientes} tickets pendientes.");

                            _ultimaAlerta = DateTime.Now;
                            _saturacionActiva = true;
                        }
                    }
                    else
                    {
                        if (_saturacionActiva)
                        {
                            _ticketService.RegistrarEvento(
                                "INFO",
                                "La saturación del sistema ha finalizado.");

                            _saturacionActiva = false;
                        }
                    }

                    Thread.Sleep(5000);
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
                catch (Exception)
                {
                    // Silencioso para no romper el menú.
                }
            }
        }
    }
}
