using Ass1.Models;

namespace Ass1.Data
{
    public class DbInitializer
    {
        public static void Initialize(EventDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Events.Any()) return;

            var events = new List<Event>
            {
                new Event
                {
                    Title = "Tech Conference 2025",
                    Name = "Tech Conference",
                    Date = new DateTime(2025, 9, 15, 9, 0, 0),
                    Location = "Ottawa Convention Centre",
                    Attendees = new List<Attendee>
                    {
                        new Attendee { Name = "Alice Martin", Email = "alice@example.com" },
                        new Attendee { Name = "Bob Chen", Email = "bob@example.com" }
                    }
                },
                new Event
                {
                    Title = "ASP.NET Workshop",
                    Name = "ASP.NET Workshop",
                    Date = new DateTime(2025, 10, 3, 13, 0, 0),
                    Location = "Algonquin College, Room T117",
                    Attendees = new List<Attendee>
                    {
                        new Attendee { Name = "Carol White", Email = "carol@example.com" },
                        new Attendee { Name = "David Lee", Email = "david@example.com" }
                    }
                },
                new Event
                {
                    Title = "Cloud & DevOps Summit",
                    Name = "Cloud Summit",
                    Date = new DateTime(2025, 11, 20, 8, 30, 0),
                    Location = "Shaw Centre, Hall B",
                    Attendees = new List<Attendee>
                    {
                        new Attendee { Name = "Eva Brown", Email = "eva@example.com" },
                        new Attendee { Name = "Frank Stone", Email = "frank@example.com" }
                    }
                }
            };

            context.Events.AddRange(events);
            context.SaveChanges();
        }
    }
}