using ForestChurches.Components.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Security.Policy;

namespace ForestChurches.Models
{
    public class EventsModel
    {
        [Column("EventID")]
        public Guid ID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Event Date")]
        public DateOnly Date {  get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Event Start")]
        public TimeOnly StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Event Finish")]
        public TimeOnly EndTime { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description of event")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Location of event")]
        public string Address { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Relavent Links for event (I.E. Booking link, more info...")]
        public string Link { get; set; }

        public byte[] ImageArray { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string User { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Church { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Repeats { get; set; }

        [NotMapped]
        [DataType(DataType.Text)]
        public string SelectedUsername { get; set; }
    }

    public class OrganizedEvents
    {
        public EventsModel Events { get; set; }
        public ChurchAccount User { get; set; }
    }
}
