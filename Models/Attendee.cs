using System.ComponentModel.DataAnnotations;

namespace Ass1.Models
{
    public class Attendee
    {
     public int Id { get; set; }

     public int EventId { get; set; }
     public Event? Event { get; set; }
        [Required(ErrorMessage = "Name is required.")]
     [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
     public string Name {
          get; 
          set; 
     } = string.Empty;

     [Required(ErrorMessage = "Email is required.")]
     [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
     public string Email { 
            get;
            set;
     } = string.Empty;

}
}
