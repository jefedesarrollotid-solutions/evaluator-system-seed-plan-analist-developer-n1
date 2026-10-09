using SemillaTickets.Web.Models;

namespace SemillaTickets.Web.Helpers;

public static class SlaHelper
{
    /// <summary>SLA de un ticket crítico: debe resolverse en un máximo de 4 horas.</summary>
    public const int HorasSlaCritico = 4;

    /// <summary>
    /// Indica si el ticket incumplió su SLA. Solo aplica a la prioridad Critica.
    /// Si el ticket ya tiene FechaCierre se mide hasta esa fecha; si sigue abierto, hasta <paramref name="ahora"/>.
    /// </summary>
    public static bool VenceSla(Ticket t, DateTime ahora)
    {
        // Se compara con la constante del catálogo ("Critica", sin tilde), igual que los datos guardados.
        if (t.Prioridad != Prioridades.Critica)
        {
            return false;
        }

        var fin = t.FechaCierre ?? ahora;

        // TotalHours devuelve el total de horas transcurridas (por ejemplo 26.5);
        // .Hours solo devolvía el componente de 0 a 23 del TimeSpan, por eso un ticket de 2 días daba 0 horas.
        double horas = (fin - t.FechaCreacion).TotalHours;

        return horas > HorasSlaCritico;
    }
}
