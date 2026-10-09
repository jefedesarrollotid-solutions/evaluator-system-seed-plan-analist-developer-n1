using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemillaTickets.Web.Data;
using SemillaTickets.Web.Helpers;
using SemillaTickets.Web.Models;

namespace SemillaTickets.Web.Controllers;

public class TicketsController : Controller
{
    private readonly AppDbContext _db;

    public TicketsController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index(string? buscar)
    {
        List<Ticket> tickets;

        if (string.IsNullOrEmpty(buscar))
        {
            tickets = _db.Tickets.ToList();
        } 
        else
        {
            tickets = _db.Tickets
                .FromSqlRaw("SELECT * FROM Tickets WHERE Titulo LIKE '%" + buscar + "%'")
                .ToList();
        }

        foreach (var t in tickets)
        {
            ViewData[$"sla_{t.Id}"] = SlaHelper.VenceSla(t, DateTime.Now);
        }

        return View(tickets);
    }

    public IActionResult Edit(int id)
    {
        var ticket = _db.Tickets.Find(id);
        if (ticket == null)
        {
            return NotFound();
        }

        return View(ticket);
    }

    [HttpPost]
    public IActionResult Edit(int id, Ticket form)
    {
        var ticket = _db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == id);
        if (ticket == null)
        {
            return NotFound();
        }

        ticket.Titulo = form.Titulo;
        ticket.Descripcion = form.Descripcion;
        ticket.Prioridad = form.Prioridad;
        ticket.Estado = form.Estado;
        ticket.Responsable = form.Responsable;
        ticket.FechaCreacion = form.FechaCreacion;
        ticket.FechaCierre = form.FechaCierre;

        return RedirectToAction(nameof(Index));
    }

    

}
