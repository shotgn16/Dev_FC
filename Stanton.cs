using ForestChurches.Models;

namespace ForestChurches
{
    public class Stanton
    {
        public static Stanton StaticConfiguration = new Stanton();
        public List<PlacesAPI.Place> churchs { get; set; }
    }
}
