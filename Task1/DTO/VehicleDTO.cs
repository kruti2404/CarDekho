
using System.ComponentModel;

namespace Task1.DTO
{
    public class VehicleDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        [DisplayName("Model Year")]
        public int ModelYear { get; set; }
        public decimal Price { get; set; }
        public int Rating { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public int Quantity { get; set; }
        public string? ColoursNames { get; set; }
    }
}

