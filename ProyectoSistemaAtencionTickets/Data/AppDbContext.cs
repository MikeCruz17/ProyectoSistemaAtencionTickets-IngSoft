using Microsoft.EntityFrameworkCore;
using ProyectoSistemaAtencionTickets.Models;

namespace ProyectoSistemaAtencionTickets.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<EventoSistema> Eventos => Set<EventoSistema>();
        public DbSet<Agente> Agentes => Set<Agente>();
        public DbSet<EstadoTicket> Estados => Set<EstadoTicket>();
        public DbSet<Prioridad> Prioridades => Set<Prioridad>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EstadoTicket>(entity =>
            {
                entity.ToTable("Estados");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            modelBuilder.Entity<Prioridad>(entity =>
            {
                entity.ToTable("Prioridades");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(p => p.Nombre).IsUnique();
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("Tickets");
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Titulo)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(t => t.Descripcion)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(t => t.AgenteAsignado)
                    .HasMaxLength(100);

                entity.Property(t => t.FechaCreacion)
                    .IsRequired();

                entity.HasOne(t => t.Estado)
                    .WithMany(e => e.Tickets)
                    .HasForeignKey(t => t.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Prioridad)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(t => t.PrioridadId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EventoSistema>(entity =>
            {
                entity.ToTable("Eventos");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.FechaHora)
                    .IsRequired();
            });

            modelBuilder.Entity<Agente>(entity =>
            {
                entity.ToTable("Agentes");
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(a => a.Disponible)
                    .IsRequired();
            });

            modelBuilder.Entity<EstadoTicket>().HasData(
                new EstadoTicket { Id = 1, Nombre = "Pendiente" },
                new EstadoTicket { Id = 2, Nombre = "En Proceso" },
                new EstadoTicket { Id = 3, Nombre = "Cerrado" }
            );

            modelBuilder.Entity<Prioridad>().HasData(
                new Prioridad { Id = 1, Nombre = "Alta" },
                new Prioridad { Id = 2, Nombre = "Media" },
                new Prioridad { Id = 3, Nombre = "Baja" }
            );

            modelBuilder.Entity<Agente>().HasData(
                new Agente { Id = 1, Nombre = "Agente 1", Disponible = true },
                new Agente { Id = 2, Nombre = "Agente 2", Disponible = true }
            );
        }
    }
}
