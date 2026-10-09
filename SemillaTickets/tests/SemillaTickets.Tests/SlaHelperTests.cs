using SemillaTickets.Web.Helpers;
using SemillaTickets.Web.Models;

namespace SemillaTickets.Tests;

public class SlaHelperTests
{
    private static readonly DateTime Creado = new(2026, 10, 7, 18, 0, 0);

    private static Ticket Critico(DateTime? fechaCierre = null) => new()
    {
        Id = 7,
        Titulo = "Error intermitente al iniciar sesión",
        Prioridad = Prioridades.Critica,
        Estado = fechaCierre == null ? Estados.Abierto : Estados.Resuelto,
        FechaCreacion = Creado,
        FechaCierre = fechaCierre
    };

    [Fact]
    public void Critico_AbiertoHaceMasDeUnDia_VenceSla()
    {
        // Con .Hours (componente 0-23) este caso devolvía 0 horas y nunca marcaba el SLA.
        Assert.True(SlaHelper.VenceSla(Critico(), Creado.AddDays(2)));
    }

    [Fact]
    public void Critico_AbiertoHaceCincoHoras_VenceSla()
    {
        Assert.True(SlaHelper.VenceSla(Critico(), Creado.AddHours(5)));
    }

    [Fact]
    public void Critico_AbiertoHaceTresHoras_NoVenceSla()
    {
        Assert.False(SlaHelper.VenceSla(Critico(), Creado.AddHours(3)));
    }

    [Fact]
    public void Critico_ExactamenteCuatroHoras_NoVenceSla()
    {
        Assert.False(SlaHelper.VenceSla(Critico(), Creado.AddHours(4)));
    }

    [Fact]
    public void Critico_ResueltoEnDosHoras_NoVenceSlaAunquePasenDias()
    {
        var ticket = Critico(fechaCierre: Creado.AddHours(2));

        Assert.False(SlaHelper.VenceSla(ticket, Creado.AddDays(3)));
    }

    [Fact]
    public void Critico_ResueltoEnSeisHoras_VenceSla()
    {
        var ticket = Critico(fechaCierre: Creado.AddHours(6));

        Assert.True(SlaHelper.VenceSla(ticket, Creado.AddDays(3)));
    }

    [Theory]
    [InlineData(Prioridades.Alta)]
    [InlineData(Prioridades.Media)]
    [InlineData(Prioridades.Baja)]
    public void NoCritico_AunqueLleveDias_NoVenceSla(string prioridad)
    {
        var ticket = Critico();
        ticket.Prioridad = prioridad;

        Assert.False(SlaHelper.VenceSla(ticket, Creado.AddDays(5)));
    }
}
