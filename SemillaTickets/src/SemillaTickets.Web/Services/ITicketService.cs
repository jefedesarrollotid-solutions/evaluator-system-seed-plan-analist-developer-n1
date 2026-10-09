using SemillaTickets.Web.Models;

namespace SemillaTickets.Web.Services;

/// <summary>
/// Lógica de negocio y acceso a datos de los tickets.
/// El controlador solo habla con esta interfaz, nunca con el DbContext.
/// </summary>
public interface ITicketService
{
    /// <summary>Lista los tickets, filtrando opcionalmente por texto en el título.</summary>
    List<Ticket> Listar(string? buscar);

    /// <summary>Devuelve el ticket con ese Id, o null si no existe.</summary>
    Ticket? Obtener(int id);

    /// <summary>
    /// Actualiza título, descripción y prioridad del ticket indicado.
    /// Devuelve null si el ticket no existe.
    /// Lanza <see cref="System.ComponentModel.DataAnnotations.ValidationException"/> si los datos no son válidos.
    /// </summary>
    Ticket? Actualizar(int id, Ticket datos);
}
