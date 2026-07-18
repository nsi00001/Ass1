using Ass1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ass1.Controllers
{
    public class EventManagerController : Controller
    {
        private static readonly List<Event> _events = new List<Event>
        {
            new Event
            {
                Id = 1,
                Title = "Tech Conference 2025",
                Date = new DateTime(2025, 9, 15, 9, 0, 0),
                Location = "Ottawa Convention Centre",
                Attendees = new List<Attendee>
                {
                    new Attendee { Name = "Alice Martin", Email = "alice@example.com" },
                    new Attendee { Name = "Bob Chen",    Email = "bob@example.com"   }
                }
            },
            new Event
            {
                Id = 2,
                Title = "ASP.NET Workshop",
                Date = new DateTime(2025, 10, 3, 13, 0, 0),
                Location = "Algonquin College, Room T117",
                Attendees = new List<Attendee>()
            },
            new Event
            {
                Id = 3,
                Title = "Cloud & DevOps Summit",
                Date = new DateTime(2025, 11, 20, 8, 30, 0),
                Location = "Shaw Centre, Hall B",
                Attendees = new List<Attendee>
                {
                    new Attendee { Name = "Carol White", Email = "carol@example.com" }
                }
            }
        };

        // GET: /EventManager/Index
        // Passes event list via both ViewData and the model (demonstrates both techniques)
        public IActionResult Index()
        {
            ViewData["PageTitle"] = "Event Manager";
            ViewData["EventCount"] = _events.Count;

            return View(_events);
        }

        // GET: /EventManager/ManageAttendees/5
        public IActionResult ManageAttendees(int id)
        {
            // LINQ FirstOrDefault to locate the event by id
            Event? ev = _events.FirstOrDefault(e => e.Id == id);

            if (ev == null)
            {
                return NotFound($"No event found with id {id}.");
            }

            // Pass event name to the view via ViewData
            ViewData["EventName"] = ev.Title;

            return View(ev);
        }

        // POST: /EventManager/ManageAttendees/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ManageAttendees(int id, Attendee attendee)
        {
            Event? ev = _events.FirstOrDefault(e => e.Id == id);

            if (ev == null)
            {
                return NotFound($"No event found with id {id}.");
            }

            if (ModelState.IsValid)
            {
                ev.Attendees.Add(attendee);

                // PRG pattern — redirect after POST to avoid duplicate submissions on refresh
                return RedirectToAction(nameof(ManageAttendees), new { id });
            }

            // Validation failed — redisplay form with error messages
            ViewData["EventName"] = ev.Title;
            return View(ev);
        }

    }
}
