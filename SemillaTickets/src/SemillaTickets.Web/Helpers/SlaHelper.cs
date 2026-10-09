using SemillaTickets.Web.Models;

namespace SemillaTickets.Web.Helpers;

public static class SlaHelper
{
    public static bool VenceSla(Ticket t, DateTime ahora)
    {
        int horas = (ahora - t.FechaCreacion).Hours;
        return t.Prioridad == "Crítica" && horas > 4;
    }
}
