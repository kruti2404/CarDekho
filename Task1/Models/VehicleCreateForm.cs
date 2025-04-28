using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Task1.Models
{
    public class VehicleCreateForm
    {

        public int? VehicleId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Model Year is required")]
        [DisplayName("Model Year")]
        [Range(1800, 2025, ErrorMessage = "Model Year must be between 1800 and 2025")]
        public int ModelYear { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(2000000, 200000000, ErrorMessage = "Price is not correct")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(0, 5, ErrorMessage = "Rating must be between 0-5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Brand Name is required")]
        [DisplayName("Brand Name")]
        public string BrandName { get; set; }

        public IEnumerable<string>? BrandList { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [DisplayName("Category Name")]
        public string CategoryName { get; set; }

        public IEnumerable<string>? CategoriesList { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be at least 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Colours are required")]
        [DisplayName("Colours Name")]
        public string[] SelectedColours { get; set; }

        public IEnumerable<string>? ColoursList { get; set; }
    }
}
