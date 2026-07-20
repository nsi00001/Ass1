namespace Ass1.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? BannerUrl { get; set; }
        public List<Attendee> Attendees { get; set; } = new List<Attendee>();   
    }
}
