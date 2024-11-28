using ForestChurches.Components.Users;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace ForestChurches.Models
{
    public class ChurchInformation
    {
        [Column("ID")]
        public string ID { get; set; }
        public string ChurchAccountId { get; set; }
        public ChurchAccount ChurchAccount { get; set; }

        // Collection of serviceTimes
        public ICollection<ServiceTimes> ServiceTimes { get; set; } = new List<ServiceTimes>();

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Denomination")]
        public string Denominaion { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Congregation")]
        public string Congregation { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Churchsuite")]
        public string Churchsuite { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Website")]
        public string Website { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Parking")]
        public bool Parking { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Restrooms")]
        public bool Restrooms { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Wheelchair Access")]
        public bool WheelchairAccess { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Wifi")]
        public bool Wifi { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Refreshments")]
        public bool Refreshments { get; set; }

        public List<string> Activities { get; set; } = new List<string>();
    }
}
