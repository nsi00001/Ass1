using Ass1.Data;
using Ass1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Ass1.Controllers
{
    [Route("events/{eventId}/attendees")]
    public class AttendeesController : Controller
    {
        private readonly EventDbContext _context;

        public AttendeesController(EventDbContext context)
        {
            _context = context;
        }

        // GET: /events/5/attendees
        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Index(int eventId)
        {
            var ev = await _context.Events
                .Include(e => e.Attendees)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null) return NotFound();

            ViewData["EventTitle"] = ev.Title;
            ViewData["EventId"] = eventId;
            return View(ev.Attendees);
        }

        // GET: /events/5/attendees/create
        [Authorize(Roles = "Organizer")]
        [HttpGet("create")]
        public IActionResult Create(int eventId)
        {
            ViewData["EventId"] = eventId;
            return View();
        }

        // POST: /events/5/attendees/create
        [Authorize(Roles = "Organizer")]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int eventId,
            [Bind("Id,Name,Email")] Attendee model)
        {
            if (ModelState.IsValid)
            {
                model.EventId = eventId;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { eventId });
            }
            ViewData["EventId"] = eventId;
            return View(model);
        }

        // GET: /events/5/attendees/edit/3
        [Authorize(Roles = "Organizer")]
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int eventId, int id)
        {
            var attendee = await _context.Attendees
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId);

            if (attendee == null) return NotFound();

            ViewData["EventId"] = eventId;
            return View(attendee);
        }

        // POST: /events/5/attendees/edit/3
        [Authorize(Roles = "Organizer")]
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int eventId,
            int id,
            [Bind("Id,Name,Email")] Attendee model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    model.EventId = eventId;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendeeExists(model.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index), new { eventId });
            }
            ViewData["EventId"] = eventId;
            return View(model);
        }

        // GET: /events/5/attendees/delete/3
        [Authorize(Roles = "Organizer")]
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int eventId, int id)
        {
            var attendee = await _context.Attendees
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId);

            if (attendee == null) return NotFound();

            ViewData["EventId"] = eventId;
            return View(attendee);
        }

        // POST: /events/5/attendees/delete/3
        [Authorize(Roles = "Organizer")]
        [HttpPost("delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int eventId, int id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee != null)
            {
                _context.Attendees.Remove(attendee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { eventId });
        }

        private bool AttendeeExists(int id)
        {
            return _context.Attendees.Any(a => a.Id == id);
        }
    }
}