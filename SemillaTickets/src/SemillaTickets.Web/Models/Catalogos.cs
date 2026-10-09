namespace SemillaTickets.Web.Models;

/// <summary>Valores permitidos para Ticket.Prioridad (coinciden con los datos semilla, sin tilde).</summary>
public static class Prioridades
{
    public const string Baja = "Baja";
    public const string Media = "Media";
    public const string Alta = "Alta";
    public const string Critica = "Critica";

    public static readonly string[] Todas = { Baja, Media, Alta, Critica };
}

/// <summary>Valores permitidos para Ticket.Estado.</summary>
public static class Estados
{
    public const string Abierto = "Abierto";
    public const string EnProceso = "En proceso";
    public const string Resuelto = "Resuelto";
    public const string Cerrado = "Cerrado";

    public static readonly string[] Todos = { Abierto, EnProceso, Resuelto, Cerrado };
}
