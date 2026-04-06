using ProyectoSistemaAtencionTickets.Concurrency;
using ProyectoSistemaAtencionTickets.Data;
using ProyectoSistemaAtencionTickets.Exceptions;
using ProyectoSistemaAtencionTickets.Services;

namespace ProyectoSistemaAtencionTickets
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==============================================");
            Console.WriteLine(" Sistema de Atención de Tickets en Tiempo Real");
            Console.WriteLine("==============================================");

            string connectionString =
                "Server=localhost;Database=ProyectoFinalTicketsDB;Trusted_Connection=True;TrustServerCertificate=True;";

            var contextFactory = new AppDbContextFactory(connectionString);

            try
            {
                using (var context = contextFactory.CreateDbContext())
                {
                    context.Database.EnsureCreated();
                }

                var monitor = new TicketMonitor();
                var ticketService = new TicketService(contextFactory);

                ticketService.OnEventoImportante += mensaje =>
                {
                    // No mostrar eventos automáticos en consola.
                    // Se consultan desde el menú.
                };

                var generador = new TicketGenerator(monitor, ticketService);
                var supervisor = new RealtimeSupervisor(monitor, ticketService);

                Thread hiloGenerador = new(new ThreadStart(generador.Ejecutar));
                Thread hiloSupervisor = new(new ThreadStart(supervisor.Ejecutar));
                Thread agente1 = new(new ThreadStart(new AgentWorker("Agente 1", monitor, ticketService).Ejecutar));
                Thread agente2 = new(new ThreadStart(new AgentWorker("Agente 2", monitor, ticketService).Ejecutar));

                hiloGenerador.Start();
                hiloSupervisor.Start();
                agente1.Start();
                agente2.Start();

                bool salir = false;

                while (!salir)
                {
                    Console.WriteLine();
                    Console.WriteLine("--------------- MENÚ ---------------");
                    Console.WriteLine("1. Crear ticket manual");
                    Console.WriteLine("2. Listar tickets");
                    Console.WriteLine("3. Ver eventos");
                    Console.WriteLine("4. Ver cantidad de tickets pendientes");
                    Console.WriteLine("5. Salir");
                    Console.Write("Seleccione una opción: ");

                    string? opcion = Console.ReadLine();

                    try
                    {
                        switch (opcion)
                        {
                            case "1":
                                CrearTicketManual(ticketService, monitor);
                                break;

                            case "2":
                                ListarTickets(ticketService);
                                break;

                            case "3":
                                ListarEventos(ticketService);
                                break;

                            case "4":
                                Console.WriteLine($"Tickets pendientes en cola: {monitor.CantidadPendiente()}");
                                break;

                            case "5":
                                salir = true;
                                break;

                            default:
                                Console.WriteLine("Opción inválida.");
                                break;
                        }
                    }
                    catch (TicketInvalidoException ex)
                    {
                        Console.WriteLine($"[ERROR DE VALIDACIÓN] {ex.Message}");
                    }
                    catch (DatabaseException ex)
                    {
                        Console.WriteLine($"[ERROR DE BASE DE DATOS] {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR GENERAL] {ex.Message}");
                    }
                }

                generador.Detener();
                supervisor.Detener();

                hiloGenerador.Interrupt();
                hiloSupervisor.Interrupt();
                agente1.Interrupt();
                agente2.Interrupt();

                hiloGenerador.Join();
                hiloSupervisor.Join();
                agente1.Join();
                agente2.Join();

                Console.WriteLine("Sistema finalizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fatal al iniciar el sistema: {ex.Message}");
            }
        }

        static void CrearTicketManual(TicketService ticketService, TicketMonitor monitor)
        {
            Console.Write("Título: ");
            string titulo = Console.ReadLine() ?? "";

            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine() ?? "";

            Console.WriteLine("Prioridad:");
            Console.WriteLine("1. Alta");
            Console.WriteLine("2. Media");
            Console.WriteLine("3. Baja");
            Console.Write("Seleccione prioridad: ");

            string? prioridadTexto = Console.ReadLine();

            if (!int.TryParse(prioridadTexto, out int prioridadId))
                throw new TicketInvalidoException("La prioridad debe ser un número válido.");

            var ticket = ticketService.CrearTicket(titulo, descripcion, prioridadId);
            monitor.AgregarTicket(ticket);

            Console.WriteLine($"Ticket manual creado correctamente con ID #{ticket.Id}");
        }

        static void ListarTickets(TicketService ticketService)
        {
            var tickets = ticketService.ObtenerTodos();

            Console.WriteLine();
            Console.WriteLine("--------- LISTA DE TICKETS ---------");

            if (tickets.Count == 0)
            {
                Console.WriteLine("No hay tickets registrados.");
                return;
            }

            foreach (var ticket in tickets)
            {
                Console.Write($"[{ticket.Id}] {ticket.Titulo} | Estado: ");

                // ===== COLOR ESTADO =====
                ConsoleColor colorOriginal = Console.ForegroundColor;
                Console.ForegroundColor = ObtenerColorEstado(ticket.Estado.Nombre);
                Console.ForegroundColor = colorOriginal;

                // ===== PRIORIDAD =====
                Console.Write(" | Prioridad: ");
                Console.ForegroundColor = ObtenerColorPrioridad(ticket.Prioridad.Nombre);
                Console.Write(ticket.Prioridad.Nombre);
                Console.ForegroundColor = colorOriginal;

                // ===== RESTO =====
                Console.WriteLine($" | Agente: {ticket.AgenteAsignado ?? "Sin asignar"} | Fecha: {ticket.FechaCreacion}");
            }
        }

        static void ListarEventos(TicketService ticketService)
        {
            var eventos = ticketService.ObtenerEventos();

            Console.WriteLine();
            Console.WriteLine("--------- HISTORIAL DE EVENTOS ---------");

            if (eventos.Count == 0)
            {
                Console.WriteLine("No hay eventos registrados.");
                return;
            }

            foreach (var evento in eventos)
            {
                ConsoleColor colorOriginal = Console.ForegroundColor;
                Console.ForegroundColor = ObtenerColorEvento(evento.Tipo);

                Console.WriteLine($"[{evento.FechaHora:yyyy-MM-dd HH:mm:ss}] {evento.Tipo} - {evento.Descripcion}");

                Console.ForegroundColor = colorOriginal;
            }
        }

        static ConsoleColor ObtenerColorEvento(string tipoEvento)
        {
            return tipoEvento.ToUpper() switch
            {
                "CIERRE" => ConsoleColor.Green,
                "ASIGNACION" => ConsoleColor.Yellow,
                "ALERTA" => ConsoleColor.Red,
                "CREACION" => ConsoleColor.Cyan,
                "INFO" => ConsoleColor.Blue,
                _ => ConsoleColor.White
            };
        }

        static ConsoleColor ObtenerColorEstado(string estado)
        {
            return estado.ToUpper() switch
            {
                "CERRADO" => ConsoleColor.Green,
                "EN PROCESO" => ConsoleColor.Yellow,
                "PENDIENTE" => ConsoleColor.Cyan,
                _ => ConsoleColor.White
            };
        }

        static ConsoleColor ObtenerColorPrioridad(string prioridad)
        {
            return prioridad.ToUpper() switch
            {
                "ALTA" => ConsoleColor.Red,
                "MEDIA" => ConsoleColor.Yellow,
                "BAJA" => ConsoleColor.Green,
                _ => ConsoleColor.White
            };
        }
    }
}