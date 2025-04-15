namespace Task1.Models
{
    public class Colours
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Vehicles> Vehicles { get; set; }
    }
}
