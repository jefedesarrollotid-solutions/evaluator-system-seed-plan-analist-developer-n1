namespace SemillaTickets.Web.Models;

public class Ticket
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Prioridad { get; set; } = "Media";
    public string Estado { get; set; } = "Abierto";
    public string? Responsable { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaCierre { get; set; }
}
