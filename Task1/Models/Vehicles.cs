namespace Task1.Models
{
    public class Vehicles
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ModelYear { get; set; }
        public decimal Price { get; set; }
        public int Rating { get; set; }
        public int? BrandID { get; set; }
        public Brands? Brands { get; set; }
        public int? CategoryId { get; set; }
        public Categories? Categories { get; set; }
        public int? StockId { get; set; }
        public Stocks? Stocks { get; set; }
        public ICollection<Colours>? Colours { get; set; }
    }
}
