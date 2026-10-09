using System.ComponentModel.DataAnnotations;

namespace SemillaTickets.Web.Models;

public class Ticket
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 100 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "La prioridad es obligatoria.")]
    [RegularExpression("^(Baja|Media|Alta|Critica)$", ErrorMessage = "La prioridad debe ser Baja, Media, Alta o Critica.")]
    public string Prioridad { get; set; } = "Media";

    public string Estado { get; set; } = "Abierto";
    public string? Responsable { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaCierre { get; set; }
}
