namespace Task1.Models
{
    public class Stocks
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int Quantity { get; set; }
        public Vehicles Vehicle { get; set; }
    }
}
