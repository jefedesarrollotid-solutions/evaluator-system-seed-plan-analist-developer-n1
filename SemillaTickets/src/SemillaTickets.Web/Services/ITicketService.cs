using SemillaTickets.Web.Models;

namespace SemillaTickets.Web.Services;

/// <summary>
/// Lógica de negocio y acceso a datos de los tickets.
/// El controlador solo habla con esta interfaz, nunca con el DbContext.
/// </summary>
public interface ITicketService
{
    /// <summary>
    /// Lista los tickets del más reciente al más antiguo.
    /// Los filtros son opcionales y combinables: texto en el título, estado y prioridad.
    /// </summary>
    List<Ticket> Listar(string? buscar, string? estado, string? prioridad);

    /// <summary>Devuelve el ticket con ese Id, o null si no existe.</summary>
    Ticket? Obtener(int id);

    /// <summary>
    /// Actualiza título, descripción y prioridad del ticket indicado.
    /// Devuelve null si el ticket no existe.
    /// Lanza <see cref="System.ComponentModel.DataAnnotations.ValidationException"/> si los datos no son válidos.
    /// </summary>
    Ticket? Actualizar(int id, Ticket datos);

    /// <summary>
    /// Cambia el estado del ticket. Reglas:
    /// un ticket Cerrado no puede cambiar de estado (lanza <see cref="InvalidOperationException"/>);
    /// al pasar a Resuelto o Cerrado se registra FechaCierre con <paramref name="ahora"/>.
    /// Devuelve null si el ticket no existe.
    /// Lanza <see cref="System.ComponentModel.DataAnnotations.ValidationException"/> si el estado no es válido.
    /// </summary>
    Ticket? CambiarEstado(int id, string nuevoEstado, DateTime ahora);
}
