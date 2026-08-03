using Ass1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ass1.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<EventDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.EnsureCreated();

            // Seed roles
            if (!await roleManager.RoleExistsAsync("Organizer"))
                await roleManager.CreateAsync(new IdentityRole("Organizer"));

            if (!await roleManager.RoleExistsAsync("Attendee"))
                await roleManager.CreateAsync(new IdentityRole("Attendee"));

            // Seed organizer user
            if (await userManager.FindByEmailAsync("organizer@example.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "organizer@example.com",
                    Email = "organizer@example.com"
                };
                await userManager.CreateAsync(user, "Organizer@123");
                await userManager.AddToRoleAsync(user, "Organizer");
            }

            // Seed regular user
            if (await userManager.FindByEmailAsync("user@example.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "user@example.com",
                    Email = "user@example.com"
                };
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, "Attendee");
            }

            // Seed events
            if (context.Events.Any()) return;

            var events = new List<Event>
            {
                new Event
                {
                    Title = "Tech Conference 2026",
                    Name = "Tech Conference",
                    Date = new DateTime(2026, 3, 27),
                    Location = "Ottawa Convention Centre",
                    Attendees = new List<Attendee>
                    {
                        new Attendee { Name = "Alice Smith", Email = "alice@example.com" },
                        new Attendee { Name = "Bob Jones", Email = "bob@example.com" }
                    }
                },
                new Event
                {
                    Title = "ASP.NET Workshop",
                    Name = "ASP.NET Workshop",
                    Date = new DateTime(2026, 4, 10),
                    Location = "Algonquin College, Room T117",
                    Attendees = new List<Attendee>
                    {
                        new Attendee { Name = "Carol White", Email = "carol@example.com" },
                        new Attendee { Name = "David Lee", Email = "david@example.com" }
                    }
                },
                new Event
                {
                    Title = "EF Core Bootcamp",
                    Name = "EF Core Bootcamp",
                    Date = new DateTime(2026, 4, 26),
                    Location = "Online",
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