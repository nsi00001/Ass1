using Ass1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Ass1.Data
{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Attendee> Attendees { get; set; }
    }
}