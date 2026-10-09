using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
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

    public List<Ticket> Listar(string? buscar)
    {
        if (string.IsNullOrEmpty(buscar))
        {
            return _db.Tickets.ToList();
        }

        return _db.Tickets
            .FromSqlRaw("SELECT * FROM Tickets WHERE Titulo LIKE '%" + buscar + "%'")
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

    /// <summary>
    /// Aplica las Data Annotations del modelo también en la capa de servicio,
    /// para que las reglas se cumplan aunque no se llegue desde el controlador (por ejemplo, desde las pruebas).
    /// </summary>
    private static void Validar(Ticket datos)
    {
        Validator.ValidateObject(datos, new ValidationContext(datos), validateAllProperties: true);
    }
}
