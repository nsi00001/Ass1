using Ass1.Data;
using Ass1.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ass1.Controllers
{
    [Route("events")]
    public class EventsController : Controller
    {
        private readonly EventDbContext _context;
        private readonly IConfiguration _config;

        public EventsController(EventDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private async Task<string> UploadToBlob(IFormFile file)
        {
            var connectionString = _config["AzureBlob:ConnectionString"];
            var containerName = _config["AzureBlob:ContainerName"];

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create container if it doesn't exist, with public access
            await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            // Use a unique filename to avoid collisions
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            // Return the public URL saved into BannerUrl
            return blobClient.Uri.ToString();
        }

        // GET: /events
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .Include(e => e.Attendees)
                .AsNoTracking()
                .ToListAsync();

            ViewData["PageTitle"] = "Event Manager";
            ViewData["EventCount"] = events.Count;
            return View(events);
        }

        // GET: /events/details/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ev = await _context.Events
                .Include(e => e.Attendees)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();
            return View(ev);
        }


        // GET: /events/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /events/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event model, IFormFile? bannerImage)
        {
            // Upload file FIRST (before ModelState validation)
            if (bannerImage != null && bannerImage.Length > 0)
            {
                try
                {
                    model.BannerUrl = await UploadToBlob(bannerImage);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to upload banner: {ex.Message}");
                    return View(model);
                }
            }
            else
            {
                model.BannerUrl = null;
            }

            // THEN validate the model
            if (ModelState.IsValid)
            {
                _context.Events.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /events/edit/5
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        // POST: /events/edit/5
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event model, IFormFile? bannerImage)
        {
            if (id != model.Id) return BadRequest();

            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null) return NotFound();

            // Upload new banner if provided
            if (bannerImage != null && bannerImage.Length > 0)
            {
                try
                {
                    existingEvent.BannerUrl = await UploadToBlob(bannerImage);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to upload banner: {ex.Message}");
                    return View(model);
                }
            }

            // Update properties
            existingEvent.Title = model.Title;
            existingEvent.Name = model.Name;
            existingEvent.Date = model.Date;
            existingEvent.Location = model.Location;

            if (ModelState.IsValid)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /events/delete/5
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ev = await _context.Events
                .Include(e => e.Attendees)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();
            return View(ev);
        }

        // POST: /events/delete/5
        [HttpPost("delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev != null)
            {
                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

    }
}