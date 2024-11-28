using System.ComponentModel.DataAnnotations.Schema;

namespace ForestChurches.Models
{
    public class ServiceTimes
    {
        [Column("ServiceID")]
        public Guid Id { get; set; }
        public TimeOnly Time { get; set; }
        public string Note { get; set; }

        // Foreign key to ChurchInformation
        public string ChurchInformationId { get; set; }

        [ForeignKey("ChurchInformationId")]
        public ChurchInformation ChurchInformation { get; set; }
    }
}
