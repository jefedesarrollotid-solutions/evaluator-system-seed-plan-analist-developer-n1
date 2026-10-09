using SemillaTickets.Web.Helpers;
using SemillaTickets.Web.Models;

namespace SemillaTickets.Web.Models
{
    public class
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
{
        

    }
}
