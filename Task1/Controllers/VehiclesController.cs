using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Task1.DTO;
using Task1.Models;
using Task1.Repository;
using Task1.ViewModel;


namespace Task1.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly IUnitOfWork _UOFInstance;
        public VehiclesController(IUnitOfWork UOFInstance)
        {
            _UOFInstance = UOFInstance;
        }

        [HttpGet]
        public async Task<IActionResult> Index(QueryDTO query)
        {

            IEnumerable<Brands> brands = await _UOFInstance._brandsRepository.GetAll();
            IEnumerable<Categories> categories = await _UOFInstance._categoriesRepository.GetAll();
            IEnumerable<Colours> colours = await _UOFInstance._coloursRepository.GetAll();

            var CategoriesSelect = categories.Select(cat => new SelectListItem
            {
                Value = cat.Name,
                Text = cat.Name,
            }).ToList();

            CategoriesSelect.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "-- Select Category --",
                Selected = string.IsNullOrEmpty(query.SingleFilter)
            });



            var PageList = new List<SelectListItem>
            {
                 new SelectListItem{Text="5", Value="5"},
                 new SelectListItem{Text="10", Value="10"},
                 new SelectListItem{Text="15", Value="15"},
                 new SelectListItem{Text="20", Value="20"},
            };

            PageList.FirstOrDefault(p => p.Value == query.PageSize.ToString())!.Selected = true;

            var result = await _UOFInstance._vehicleRepository.GetAll(query.PageSize, query.PageNumber, query.SearchTerm ?? "", query.SortColumn, query.SortDirection, query.SingleFilter ?? "", query.MultiFilter ?? "", query.MinPrice, query.MaxPrice, query.StockAvail.ToString() ?? "All", query.ColoursList ?? "");
            int TotalRecords = result.TotalRecords;

            var TotalPages = (int)Math.Ceiling((double)TotalRecords / query.PageSize);

            var viewModel = new VehicleViewModel
            {
                Vehicles = result.Vehicles,
                TotalRecords = TotalPages,
                Query = query,
                CategoryList = CategoriesSelect,
                ColoursOptionList = colours.Select(clr => clr.Name).ToList(),
                BrandList = brands.Select(b => b.Name),
                PageSizeList = PageList
            };

            ViewBag.SelectedValues = viewModel.Query.MultiFilter;
            return View(viewModel);

        }

    }
}
