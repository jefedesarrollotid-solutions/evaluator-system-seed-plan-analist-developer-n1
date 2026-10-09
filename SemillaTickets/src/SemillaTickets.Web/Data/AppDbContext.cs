using Microsoft.EntityFrameworkCore;
using SemillaTickets.Web.Models;

namespace SemillaTickets.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(t => t.Titulo).HasMaxLength(100).IsRequired();
            entity.Property(t => t.Descripcion).HasMaxLength(500);
            entity.Property(t => t.Prioridad).HasMaxLength(10).IsRequired();
            entity.Property(t => t.Estado).HasMaxLength(15).IsRequired();
            entity.Property(t => t.Responsable).HasMaxLength(60);

            entity.HasData(
                new Ticket { Id = 1, Titulo = "Error al emitir boleta", Prioridad = "Critica", Estado = "Resuelto", Responsable = "Equipo Dev", FechaCreacion = new DateTime(2026, 10, 1, 8, 0, 0), FechaCierre = new DateTime(2026, 10, 1, 11, 30, 0) },
                new Ticket { Id = 2, Titulo = "Lentitud en búsqueda", Prioridad = "Alta", Estado = "En proceso", Responsable = "Equipo Dev", FechaCreacion = new DateTime(2026, 10, 2, 9, 15, 0) },
                new Ticket { Id = 3, Titulo = "Cambio de texto en botón", Prioridad = "Baja", Estado = "Abierto", FechaCreacion = new DateTime(2026, 10, 3, 10, 0, 0) },
                new Ticket { Id = 4, Titulo = "Reporte mensual vacío", Prioridad = "Alta", Estado = "Cerrado", Responsable = "Equipo Dev", FechaCreacion = new DateTime(2026, 10, 3, 14, 0, 0), FechaCierre = new DateTime(2026, 10, 4, 10, 0, 0) },
                new Ticket { Id = 5, Titulo = "No carga foto de perfil", Prioridad = "Media", Estado = "Abierto", FechaCreacion = new DateTime(2026, 10, 5, 16, 20, 0) },
                new Ticket { Id = 6, Titulo = "Caída del módulo de pagos", Prioridad = "Critica", Estado = "Cerrado", Responsable = "Equipo Dev", FechaCreacion = new DateTime(2026, 10, 6, 7, 45, 0), FechaCierre = new DateTime(2026, 10, 6, 9, 15, 0) },
                new Ticket { Id = 7, Titulo = "Error intermitente al iniciar sesión", Prioridad = "Critica", Estado = "Abierto", FechaCreacion = new DateTime(2026, 10, 7, 18, 0, 0) });
        });
    }
}
