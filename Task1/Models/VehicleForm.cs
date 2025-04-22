namespace Task1.Models
{
    public class VehicleForm
    {
        public int VehicleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ModelYear { get; set; }
        public decimal Price { get; set; }
        public int Rating { get; set; }
        public string BrandName { get; set; }
        public IEnumerable<string> BrandList { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<string> CategoriesList { get; set; }
        public int Quantity { get; set; }
        public string SelectedColours { get; set; }
        public IEnumerable<string> ColoursList { get; set; }
    }
}
