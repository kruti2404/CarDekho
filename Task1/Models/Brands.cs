namespace Task1.Models
{
    public class Brands
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public List<Vehicles> Vehicle { get; set; }
    }
}
