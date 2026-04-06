using Microsoft.EntityFrameworkCore;
using ProyectoSistemaAtencionTickets.Data;
using ProyectoSistemaAtencionTickets.Exceptions;
using ProyectoSistemaAtencionTickets.Models;

namespace ProyectoSistemaAtencionTickets.Services
{
    public class TicketService
    {
        private readonly AppDbContextFactory _contextFactory;

        public event Action<string>? OnEventoImportante;

        public TicketService(AppDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Ticket CrearTicket(string titulo, string descripcion, int prioridadId, bool registrarEvento = true)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new TicketInvalidoException("El título del ticket no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(descripcion))
                throw new TicketInvalidoException("La descripción del ticket no puede estar vacía.");

            if (prioridadId < 1 || prioridadId > 3)
                throw new TicketInvalidoException("La prioridad ingresada no es válida.");

            try
            {
                using var context = _contextFactory.CreateDbContext();

                var prioridadExiste = context.Prioridades.Any(p => p.Id == prioridadId);
                if (!prioridadExiste)
                    throw new TicketInvalidoException("La prioridad no existe en la base de datos.");

                var ticket = new Ticket
                {
                    Titulo = titulo,
                    Descripcion = descripcion,
                    EstadoId = 1,
                    PrioridadId = prioridadId,
                    FechaCreacion = DateTime.Now,
                    AgenteAsignado = null
                };

                context.Tickets.Add(ticket);
                context.SaveChanges();

                context.Entry(ticket).Reference(t => t.Estado).Load();
                context.Entry(ticket).Reference(t => t.Prioridad).Load();

                if (registrarEvento)
                {
                    RegistrarEvento("CREACION", $"Se creó el ticket #{ticket.Id}: {ticket.Titulo}");
                    OnEventoImportante?.Invoke($"Nuevo ticket creado: #{ticket.Id}");
                }

                return ticket;
            }
            catch (TicketInvalidoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al crear el ticket.", ex);
            }
        }

        public void MarcarEnProceso(int ticketId, string agente)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var ticket = context.Tickets.FirstOrDefault(t => t.Id == ticketId);
                if (ticket == null)
                    throw new TicketInvalidoException($"No existe el ticket con ID {ticketId}.");

                ticket.EstadoId = 2;
                ticket.AgenteAsignado = agente;

                context.SaveChanges();

                RegistrarEvento("ASIGNACION", $"Ticket #{ticket.Id} asignado a {agente}");
                OnEventoImportante?.Invoke($"Ticket #{ticket.Id} asignado a {agente}");
            }
            catch (TicketInvalidoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al marcar ticket en proceso.", ex);
            }
        }

        public void CerrarTicket(int ticketId, string agente)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var ticket = context.Tickets.FirstOrDefault(t => t.Id == ticketId);
                if (ticket == null)
                    throw new TicketInvalidoException($"No existe el ticket con ID {ticketId}.");

                ticket.EstadoId = 3;
                ticket.AgenteAsignado = agente;

                context.SaveChanges();

                RegistrarEvento("CIERRE", $"Ticket #{ticket.Id} cerrado por {agente}");
                OnEventoImportante?.Invoke($"Ticket #{ticket.Id} cerrado");
            }
            catch (TicketInvalidoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al cerrar ticket.", ex);
            }
        }

        public List<Ticket> ObtenerTodos()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                return context.Tickets
                    .Include(t => t.Estado)
                    .Include(t => t.Prioridad)
                    .AsNoTracking()
                    .OrderBy(t => t.Id)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al obtener tickets.", ex);
            }
        }

        public List<EventoSistema> ObtenerEventos()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                return context.Eventos
                    .AsNoTracking()
                    .OrderByDescending(e => e.FechaHora)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al obtener eventos.", ex);
            }
        }

        public void RegistrarEvento(string tipo, string descripcion)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var evento = new EventoSistema
                {
                    Tipo = tipo,
                    Descripcion = descripcion,
                    FechaHora = DateTime.Now
                };

                context.Eventos.Add(evento);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al registrar evento.", ex);
            }
        }
    }
}
