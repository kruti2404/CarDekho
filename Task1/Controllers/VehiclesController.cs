using System.ComponentModel;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index([FromQuery] QueryDTO query)
        {
            Console.WriteLine("Rating is " + query.Rating);
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

            //PageList.FirstOrDefault(p => p.Value == query.PageSize.ToString())!.Selected = true;

            var result = await _UOFInstance._vehicleRepository.GetAll(query.PageSize, query.PageNumber, query.SearchTerm ?? "", query.SortColumn, query.SortDirection, query.SingleFilter ?? "", query.MultiFilter ?? "", query.MinPrice, query.MaxPrice, query.StockAvail.ToString() ?? "All", query.ColoursList ?? "", query.Rating);
            int TotalRecords = result.TotalPages;

            var TotalPages = (int)Math.Ceiling((double)TotalRecords / query.PageSize);

            var viewModel = new VehicleViewModel
            {
                Vehicles = result.Vehicles,
                TotalPages = TotalPages,
                Query = query,
                CategoryList = CategoriesSelect,
                ColoursOptionList = colours.Select(clr => clr.Name).ToList(),
                BrandList = brands.Select(b => b.Name),
                PageSizeList = PageList
            };

            //ViewBag.SelectedValues = viewModel.Query.MultiFilter;

            // Check if the request is AJAX (jQuery adds this header by default)
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // If AJAX, return ONLY the partial view containing the results
                // Ensure "_VehiclesPartial" is the correct name of your partial view
                // that holds the table and pagination.
                return PartialView("_VehiclesPartial", viewModel);
            }
            else
            {
                // If not AJAX (initial page load), return the full view
                return View(viewModel);
            }

        }
        [HttpGet]
        public async Task<IActionResult> Details(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }

            var result = await _UOFInstance._vehicleRepository.GetById(Id);

            return PartialView(result);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }

            var result = await _UOFInstance._vehicleRepository.GetById(Id);

            var vehicles = new Vehicles()
            {
                Id = result.Id,
                Name = result.Name,
                ModelYear = result.ModelYear,
                Description = result.Description,
                Price = result.Price,
                Rating = result.Rating,

            };

            return View(vehicles);
        }
        [HttpPost]
        [DisplayName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }

            var result = await _UOFInstance._vehicleRepository.GetById(Id);
            if (result == null)
            {
                return NotFound();
            }

            var vehicles = new Vehicles
            {
                Id = result.Id,
                Name = result.Name,
                ModelYear = result.ModelYear,
                Description = result.Description,
                Price = result.Price,
                Rating = result.Rating
            };

            try
            {
                _UOFInstance._vehicleRepository.Delete(vehicles);
                await _UOFInstance.Save();


                return RedirectToAction("Index", "Vehicles");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error deleting vehicle: {ex.Message}");
                return View(vehicles);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }

            var result = await _UOFInstance._vehicleRepository.GetByIdIncludingAll(Id);
            if (result == null)
            {
                return NotFound();
            }
            var brandList = (await _UOFInstance._brandsRepository.GetAll()).Select(brd => brd.Name);
            result.BrandList = brandList;

            var categoriesList = (await _UOFInstance._categoriesRepository.GetAll()).Select(clg => clg.Name);
            result.CategoriesList = categoriesList;

            var ColoursList = (await _UOFInstance._coloursRepository.GetAll()).Select(clr => clr.Name);
            result.ColoursList = ColoursList;

            var Stocks = (await _UOFInstance._stocksRepository.GetAll()).FirstOrDefault(stk => stk.VehicleId == result.VehicleId);
            result.Quantity = Stocks.Quantity;

            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(VehicleForm vehicle)
        {
            if (vehicle.VehicleId == 0 || vehicle == null || vehicle.VehicleId != vehicle.VehicleId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Values of quantity" + vehicle.Quantity);
                Console.WriteLine("Hereeeee");
                return View(vehicle);
            }

            try
            {
                var existingVehicle = (await _UOFInstance._vehicleRepository.GetAll()).FirstOrDefault(veh => veh.Id == vehicle.VehicleId);
                if (existingVehicle == null)
                {
                    return NotFound();
                }

                existingVehicle.Name = vehicle.Name;
                Console.WriteLine("BrandName from the form " + vehicle.BrandName + " " + vehicle.CategoryName + " " + vehicle.SelectedColours);
                var brands = (await _UOFInstance._brandsRepository.GetAll()).FirstOrDefault(brd => brd.Name == vehicle.BrandName);
                Console.WriteLine("Brand Id is :" + brands?.Id);
                var categories = (await _UOFInstance._categoriesRepository.GetAll()).FirstOrDefault(cat => cat.Name == vehicle.CategoryName);
                Console.WriteLine("Category Id is :" + categories?.Id);


                var stock = (await _UOFInstance._stocksRepository.GetAll()).FirstOrDefault(stk => stk.VehicleId == vehicle.VehicleId);
                Console.WriteLine("Stock Id is :" + stock?.Id);


                existingVehicle.Id = vehicle.VehicleId;
                existingVehicle.Name = vehicle.Name;
                existingVehicle.ModelYear = vehicle.ModelYear;
                existingVehicle.Description = vehicle.Description;
                existingVehicle.Price = vehicle.Price;
                existingVehicle.Rating = vehicle.Rating;
                existingVehicle.BrandID = brands?.Id;
                existingVehicle.CategoryId = categories?.Id;
                existingVehicle.StockId = stock.Id;
                existingVehicle.Stocks.Quantity = vehicle.Quantity;

                var selectedColours = (vehicle.SelectedColours).Split(",");
                var allColour = await _UOFInstance._coloursRepository.GetAll();
                var SelectEntityColour = allColour
                                                .Where(clr => selectedColours.Contains(clr.Name, StringComparer.OrdinalIgnoreCase));

                existingVehicle.Colours?.Clear();
                foreach (var colour in SelectEntityColour)
                {
                    existingVehicle.Colours?.Add(colour);
                }
                Console.WriteLine("Added Colours to the list");

                //vehicle.SelectedColours are coming as csv values and existingVehicle.Colours are the ICollection<Colours>


                await _UOFInstance.Save();

                Console.WriteLine("The edit is successfull");
                return RedirectToAction("Index", "Vehicles");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating vehicle: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the vehicle.");
                return View(vehicle);
            }
        }


    }
}
