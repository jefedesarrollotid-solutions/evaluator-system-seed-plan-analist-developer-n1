using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SemillaTickets.Web.Helpers;
using SemillaTickets.Web.Models;
using SemillaTickets.Web.Services;

namespace SemillaTickets.Web.Controllers;

public class TicketsController : Controller
{
    private readonly ITicketService _tickets;

    public TicketsController(ITicketService tickets)
    {
        _tickets = tickets;
    }

    public IActionResult Index(string? buscar)
    {
        var tickets = _tickets.Listar(buscar);

        foreach (var t in tickets)
        {
            ViewData[$"sla_{t.Id}"] = SlaHelper.VenceSla(t, DateTime.Now);
        }

        return View(tickets);
    }

    public IActionResult Edit(int id)
    {
        var ticket = _tickets.Obtener(id);
        if (ticket == null)
        {
            return NotFound();
        }

        return View(ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Ticket form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        try
        {
            var ticket = _tickets.Actualizar(id, form);
            if (ticket == null)
            {
                return NotFound();
            }
        }
        catch (ValidationException ex)
        {
            // Segunda barrera: si el servicio rechaza los datos, el error se muestra en la pantalla de edición.
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(form);
        }

        return RedirectToAction(nameof(Index));
    }
}
