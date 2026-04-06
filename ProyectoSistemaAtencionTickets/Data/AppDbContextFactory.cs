using Microsoft.EntityFrameworkCore;

namespace ProyectoSistemaAtencionTickets.Data
{
    public class AppDbContextFactory
    {
        private readonly string _connectionString;

        public AppDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            return new AppDbContext(options);
        }
    }   
}
