namespace SemillaTickets.Web.Models;

/// <summary>Datos que necesita la vista Index: los filtros aplicados, la lista y la hora de referencia para el SLA.</summary>
public class TicketsIndexViewModel
{
    public string? Buscar { get; set; }
    public string? Estado { get; set; }
    public string? Prioridad { get; set; }
    public DateTime Ahora { get; set; }
    public List<Ticket> Tickets { get; set; } = new();
}
