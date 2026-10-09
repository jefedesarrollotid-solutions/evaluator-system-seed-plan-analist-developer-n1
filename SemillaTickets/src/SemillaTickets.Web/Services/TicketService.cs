using System.ComponentModel.DataAnnotations;
using SemillaTickets.Web.Data;
using SemillaTickets.Web.Models;

namespace SemillaTickets.Web.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;

    public TicketService(AppDbContext db)
    {
        _db = db;
    }

    public List<Ticket> Listar(string? buscar, string? estado, string? prioridad)
    {
        IQueryable<Ticket> consulta = _db.Tickets;

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            // LINQ se traduce a un LIKE con parámetro: el texto del usuario nunca se concatena en el SQL.
            var texto = buscar.Trim();
            consulta = consulta.Where(t => t.Titulo.Contains(texto));
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            consulta = consulta.Where(t => t.Estado == estado);
        }

        if (!string.IsNullOrWhiteSpace(prioridad))
        {
            consulta = consulta.Where(t => t.Prioridad == prioridad);
        }

        return consulta
            .OrderByDescending(t => t.FechaCreacion)
            .ThenByDescending(t => t.Id)
            .ToList();
    }

    public Ticket? Obtener(int id)
    {
        return _db.Tickets.Find(id);
    }

    public Ticket? Actualizar(int id, Ticket datos)
    {
        datos.Titulo = datos.Titulo?.Trim() ?? string.Empty;
        Validar(datos);

        var ticket = _db.Tickets.Find(id);
        if (ticket == null)
        {
            return null;
        }

        ticket.Titulo = datos.Titulo;
        ticket.Descripcion = datos.Descripcion;
        ticket.Prioridad = datos.Prioridad;
        _db.SaveChanges();

        return ticket;
    }

    public Ticket? CambiarEstado(int id, string nuevoEstado, DateTime ahora)
    {
        if (!Estados.Todos.Contains(nuevoEstado))
        {
            throw new ValidationException($"El estado '{nuevoEstado}' no es válido.");
        }

        var ticket = _db.Tickets.Find(id);
        if (ticket == null)
        {
            return null;
        }

        if (ticket.Estado == Estados.Cerrado)
        {
            throw new InvalidOperationException("Un ticket cerrado no puede cambiar de estado.");
        }

        ticket.Estado = nuevoEstado;

        if (nuevoEstado == Estados.Resuelto || nuevoEstado == Estados.Cerrado)
        {
            // Se registra la fecha de cierre; si ya estaba resuelto se conserva la fecha original de resolución.
            ticket.FechaCierre ??= ahora;
        }
        else
        {
            // Al reabrir (Abierto / En proceso) el ticket deja de tener fecha de cierre.
            ticket.FechaCierre = null;
        }

        _db.SaveChanges();

        return ticket;
    }

    /// <summary>
    /// Aplica las Data Annotations del modelo también en la capa de servicio,
    /// para que las reglas se cumplan aunque no se llegue desde el controlador (por ejemplo, desde las pruebas).
    /// </summary>
    private static void Validar(Ticket datos)
    {
        Validator.ValidateObject(datos, new ValidationContext(datos), validateAllProperties: true);
    }
}
