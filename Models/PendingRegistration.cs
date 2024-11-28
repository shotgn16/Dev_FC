using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace ForestChurches.Models
{
    public class PendingRegistration
    {
        [Column("ID")]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Status { get; set; } // "Pending", "Completed"
        public DateTime ExpiryDate { get; set; }
        public DateTime Completed { get; set; }
    }
}
