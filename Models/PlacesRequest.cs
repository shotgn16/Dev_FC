namespace ForestChurches.Models
{
    public class PlacesRequest
    {
        public class Center
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
        }

        public class Circle
        {
            public Center center { get; set; }
            public double radius { get; set; }
        }

        public class LocationBias
        {
            public Circle circle { get; set; }
        }

        public class Root
        {
            public string textQuery { get; set; }
            public LocationBias locationBias { get; set; }
        }
    }
}
