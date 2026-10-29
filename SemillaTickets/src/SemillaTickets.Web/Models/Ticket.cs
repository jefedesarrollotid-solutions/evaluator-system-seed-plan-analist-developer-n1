using System.ComponentModel.DataAnnotations;

namespace SemillaTickets.Web.Models;

public class Ticket
{
    
    public int Id { get; set; }
    
    // Se incluye el required porque es obligatorio, y el stringlength para la cantidad de caracteres de necesarios del titulo
    [Required(ErrorMessage = "El titulo es obligatorio")]
    [StringLength (100, MinimumLength =5)]
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    //La Prioridad estaba definida en Media, se le borro y se dejó vacio,de esta manera no se crean los tickets con "Media"
    public string Prioridad { get; set; } ="";
    public string Estado { get; set; } = "Abierto";
    public string? Responsable { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaCierre { get; set; }
}
