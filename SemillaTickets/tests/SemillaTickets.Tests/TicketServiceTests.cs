using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SemillaTickets.Web.Data;
using SemillaTickets.Web.Models;
using SemillaTickets.Web.Services;

namespace SemillaTickets.Tests;

public class TicketServiceTests
{
    private static readonly DateTime Ahora = new(2026, 10, 9, 12, 0, 0);

    /// <summary>Crea un contexto EF Core InMemory. Cada prueba usa una base distinta para no compartir datos.</summary>
    private static AppDbContext CrearContexto(string nombreBase)
    {
        var opciones = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(nombreBase)
            .Options;

        return new AppDbContext(opciones);
    }

    private static Ticket NuevoTicket(int id, string estado = Estados.Abierto, string prioridad = Prioridades.Media,
        DateTime? fechaCreacion = null, DateTime? fechaCierre = null)
    {
        return new Ticket
        {
            Id = id,
            Titulo = $"Ticket de prueba {id}",
            Estado = estado,
            Prioridad = prioridad,
            FechaCreacion = fechaCreacion ?? Ahora.AddHours(-id),
            FechaCierre = fechaCierre
        };
    }

    private static TicketService CrearServicioConTickets(string nombreBase, params Ticket[] tickets)
    {
        var db = CrearContexto(nombreBase);
        db.Tickets.AddRange(tickets);
        db.SaveChanges();
        return new TicketService(db);
    }

    // ---------- Actualizar ----------

    [Fact]
    public void Actualizar_ConTituloVacio_LanzaValidationException()
    {
        var servicio = CrearServicioConTickets(nameof(Actualizar_ConTituloVacio_LanzaValidationException), NuevoTicket(1));
        var datos = new Ticket { Titulo = "", Prioridad = Prioridades.Media };

        Assert.Throws<ValidationException>(() => servicio.Actualizar(1, datos));
    }

    [Fact]
    public void Actualizar_ConTituloMenorACincoCaracteres_LanzaValidationException()
    {
        var servicio = CrearServicioConTickets(nameof(Actualizar_ConTituloMenorACincoCaracteres_LanzaValidationException), NuevoTicket(1));
        var datos = new Ticket { Titulo = "Hola", Prioridad = Prioridades.Media };

        Assert.Throws<ValidationException>(() => servicio.Actualizar(1, datos));
    }

    [Fact]
    public void Actualizar_ConPrioridadInvalida_LanzaValidationException()
    {
        var servicio = CrearServicioConTickets(nameof(Actualizar_ConPrioridadInvalida_LanzaValidationException), NuevoTicket(1));
        var datos = new Ticket { Titulo = "Título válido", Prioridad = "Urgente" };

        Assert.Throws<ValidationException>(() => servicio.Actualizar(1, datos));
    }

    [Fact]
    public void Actualizar_ConDatosValidos_GuardaLosCambiosEnLaBase()
    {
        var nombreBase = nameof(Actualizar_ConDatosValidos_GuardaLosCambiosEnLaBase);
        var servicio = CrearServicioConTickets(nombreBase, NuevoTicket(1));
        var datos = new Ticket { Titulo = "  Título corregido  ", Descripcion = "Nueva descripción", Prioridad = Prioridades.Alta };

        var resultado = servicio.Actualizar(1, datos);

        Assert.NotNull(resultado);

        // Se lee con otro contexto para comprobar que SaveChanges realmente persistió los cambios.
        using var otroContexto = CrearContexto(nombreBase);
        var guardado = otroContexto.Tickets.Single(t => t.Id == 1);
        Assert.Equal("Título corregido", guardado.Titulo);
        Assert.Equal("Nueva descripción", guardado.Descripcion);
        Assert.Equal(Prioridades.Alta, guardado.Prioridad);
    }

    [Fact]
    public void Actualizar_TicketInexistente_DevuelveNull()
    {
        var servicio = CrearServicioConTickets(nameof(Actualizar_TicketInexistente_DevuelveNull), NuevoTicket(1));
        var datos = new Ticket { Titulo = "Título válido", Prioridad = Prioridades.Baja };

        Assert.Null(servicio.Actualizar(99, datos));
    }

    // ---------- CambiarEstado ----------

    [Fact]
    public void CambiarEstado_TicketCerrado_LanzaInvalidOperationException()
    {
        var servicio = CrearServicioConTickets(nameof(CambiarEstado_TicketCerrado_LanzaInvalidOperationException),
            NuevoTicket(1, estado: Estados.Cerrado, fechaCierre: Ahora.AddHours(-1)));

        Assert.Throws<InvalidOperationException>(() => servicio.CambiarEstado(1, Estados.Abierto, Ahora));
    }

    [Theory]
    [InlineData(Estados.Resuelto)]
    [InlineData(Estados.Cerrado)]
    public void CambiarEstado_AResueltoOCerrado_RegistraFechaCierre(string nuevoEstado)
    {
        var servicio = CrearServicioConTickets($"{nameof(CambiarEstado_AResueltoOCerrado_RegistraFechaCierre)}_{nuevoEstado}",
            NuevoTicket(1, estado: Estados.EnProceso));

        var ticket = servicio.CambiarEstado(1, nuevoEstado, Ahora);

        Assert.NotNull(ticket);
        Assert.Equal(nuevoEstado, ticket.Estado);
        Assert.Equal(Ahora, ticket.FechaCierre);
    }

    [Fact]
    public void CambiarEstado_DeResueltoACerrado_ConservaLaFechaDeResolucion()
    {
        var fechaResolucion = Ahora.AddHours(-5);
        var servicio = CrearServicioConTickets(nameof(CambiarEstado_DeResueltoACerrado_ConservaLaFechaDeResolucion),
            NuevoTicket(1, estado: Estados.Resuelto, fechaCierre: fechaResolucion));

        var ticket = servicio.CambiarEstado(1, Estados.Cerrado, Ahora);

        Assert.NotNull(ticket);
        Assert.Equal(fechaResolucion, ticket.FechaCierre);
    }

    [Fact]
    public void CambiarEstado_AlReabrirUnResuelto_LimpiaFechaCierre()
    {
        var servicio = CrearServicioConTickets(nameof(CambiarEstado_AlReabrirUnResuelto_LimpiaFechaCierre),
            NuevoTicket(1, estado: Estados.Resuelto, fechaCierre: Ahora.AddHours(-1)));

        var ticket = servicio.CambiarEstado(1, Estados.Abierto, Ahora);

        Assert.NotNull(ticket);
        Assert.Equal(Estados.Abierto, ticket.Estado);
        Assert.Null(ticket.FechaCierre);
    }

    [Fact]
    public void CambiarEstado_ConEstadoInvalido_LanzaValidationException()
    {
        var servicio = CrearServicioConTickets(nameof(CambiarEstado_ConEstadoInvalido_LanzaValidationException), NuevoTicket(1));

        Assert.Throws<ValidationException>(() => servicio.CambiarEstado(1, "Pausado", Ahora));
    }

    [Fact]
    public void CambiarEstado_TicketInexistente_DevuelveNull()
    {
        var servicio = CrearServicioConTickets(nameof(CambiarEstado_TicketInexistente_DevuelveNull), NuevoTicket(1));

        Assert.Null(servicio.CambiarEstado(99, Estados.Cerrado, Ahora));
    }

    // ---------- Listar ----------

    [Fact]
    public void Listar_SinFiltros_OrdenaDelMasRecienteAlMasAntiguo()
    {
        var servicio = CrearServicioConTickets(nameof(Listar_SinFiltros_OrdenaDelMasRecienteAlMasAntiguo),
            NuevoTicket(1, fechaCreacion: Ahora.AddDays(-3)),
            NuevoTicket(2, fechaCreacion: Ahora.AddDays(-1)),
            NuevoTicket(3, fechaCreacion: Ahora.AddDays(-2)));

        var lista = servicio.Listar(null, null, null);

        Assert.Equal(new[] { 2, 3, 1 }, lista.Select(t => t.Id));
    }

    [Fact]
    public void Listar_ConEstadoYPrioridad_CombinaLosFiltros()
    {
        var servicio = CrearServicioConTickets(nameof(Listar_ConEstadoYPrioridad_CombinaLosFiltros),
            NuevoTicket(1, estado: Estados.Abierto, prioridad: Prioridades.Alta),
            NuevoTicket(2, estado: Estados.Cerrado, prioridad: Prioridades.Alta),
            NuevoTicket(3, estado: Estados.Abierto, prioridad: Prioridades.Baja),
            NuevoTicket(4, estado: Estados.Abierto, prioridad: Prioridades.Alta));

        var lista = servicio.Listar(null, Estados.Abierto, Prioridades.Alta);

        Assert.Equal(new[] { 1, 4 }, lista.Select(t => t.Id).OrderBy(id => id));
    }

    [Fact]
    public void Listar_SoloPorEstado_DevuelveTodasLasPrioridades()
    {
        var servicio = CrearServicioConTickets(nameof(Listar_SoloPorEstado_DevuelveTodasLasPrioridades),
            NuevoTicket(1, estado: Estados.Abierto, prioridad: Prioridades.Alta),
            NuevoTicket(2, estado: Estados.Cerrado, prioridad: Prioridades.Alta),
            NuevoTicket(3, estado: Estados.Abierto, prioridad: Prioridades.Baja));

        var lista = servicio.Listar(null, Estados.Abierto, null);

        Assert.Equal(2, lista.Count);
        Assert.All(lista, t => Assert.Equal(Estados.Abierto, t.Estado));
    }
}
