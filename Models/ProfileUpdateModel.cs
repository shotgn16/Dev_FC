namespace ForestChurches.Models
{
    public class ProfileUpdateModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Denomination { get; set; }
        public string Congregation { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Churchsuite { get; set; }
        public string Website { get; set; }
        public bool Parking { get; set; }
        public bool Restrooms { get; set; }
        public bool WheelchairAccess { get; set; }
        public bool Wifi { get; set; }
        public bool Refreshments { get; set; }
    }
}
