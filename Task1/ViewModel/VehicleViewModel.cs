using Microsoft.AspNetCore.Mvc.Rendering;
using Task1.DTO;

namespace Task1.ViewModel
{
    public class VehicleViewModel
    {
        public List<VehicleDTO> Vehicles { get; set; }
        public QueryDTO Query { get; set; }
        public int TotalRecords { get; set; }

        public List<SelectListItem> CategoryList { get; set; } = new();
        public List<SelectListItem> PageSizeList { get; set; } = new();
        public IEnumerable<string> BrandList { get; set; } = new List<string>();


    }
}
