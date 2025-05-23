using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Task1.DTO;
using Task1.Models;
using Task1.Repository;
using Task1.ViewModel;


namespace Task1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : Controller
    {
        private readonly IUnitOfWork _UOFInstance;
        public VehiclesController(IUnitOfWork UOFInstance)
        {
            _UOFInstance = UOFInstance;
        }

        [HttpGet("index")]
        public async Task<IActionResult> Index([FromQuery] QueryDTO query)
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

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_VehiclesPartial", viewModel);
            }
            else
            {
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

        [HttpGet("Delete/{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }

            var result = await _UOFInstance._vehicleRepository.GetById(Id);



            return View(result);
        }
        [HttpPost("Delete")]
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

                TempData["SuccessMessage"] = "Vehicle Deleted successfully!";

                return RedirectToAction("Index", "Vehicles");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting vehicle: {ex.Message}");
                return View(result);
            }
        }


        [HttpGet("Edit/{Id}")]
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
            result.BrandList = (await _UOFInstance._brandsRepository.GetAll()).Select(brd => brd.Name);

            result.CategoriesList = (await _UOFInstance._categoriesRepository.GetAll()).Select(clg => clg.Name);

            result.ColoursList = (await _UOFInstance._coloursRepository.GetAll()).Select(clr => clr.Name);

            var Stocks = (await _UOFInstance._stocksRepository.GetAll()).FirstOrDefault(stk => stk.VehicleId == result.VehicleId);
            result.Quantity = Stocks.Quantity;

            return View(result);
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(VehicleForm vehicle)
        {

            if (vehicle.VehicleId == 0 || vehicle == null || vehicle.VehicleId != vehicle.VehicleId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is not valid " + vehicle.BrandList);
                vehicle.BrandList = (await _UOFInstance._brandsRepository.GetAll()).Select(brd => brd.Name);
                vehicle.CategoriesList = (await _UOFInstance._categoriesRepository.GetAll()).Select(clg => clg.Name);
                vehicle.ColoursList = (await _UOFInstance._coloursRepository.GetAll()).Select(clr => clr.Name);
                return View(vehicle);
            }
            Console.WriteLine("Entered the post request");

            try
            {
                var existingVehicle = (await _UOFInstance._vehicleRepository.GetAll()).FirstOrDefault(veh => veh.Id == vehicle.VehicleId);
                if (existingVehicle == null)
                {
                    return NotFound();
                }

                var brands = (await _UOFInstance._brandsRepository.GetAll()).FirstOrDefault(brd => brd.Name == vehicle.BrandName);

                var categories = (await _UOFInstance._categoriesRepository.GetAll()).FirstOrDefault(cat => cat.Name == vehicle.CategoryName);

                var stock = (await _UOFInstance._stocksRepository.GetAll()).FirstOrDefault(stk => stk.VehicleId == vehicle.VehicleId);

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
                await _UOFInstance.Save();

                Console.WriteLine("The edit is successfull");
                TempData["SuccessMessage"] = "Vehicle updated successfully!";
                return RedirectToAction("Index", "Vehicles");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating vehicle: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the vehicle.");
                return View(vehicle);
            }
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var BrandList = (await _UOFInstance._brandsRepository.GetAll()).Select(brd => brd.Name);
            var CategoriesList = (await _UOFInstance._categoriesRepository.GetAll()).Select(clg => clg.Name);
            var ColoursList = (await _UOFInstance._coloursRepository.GetAll()).Select(clr => clr.Name);

            var result = new VehicleCreateForm();

            result.ColoursList = ColoursList;
            result.CategoriesList = CategoriesList;
            result.BrandList = BrandList;

            return View(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(VehicleCreateForm model)
        {
            if (ModelState.IsValid)
            {

                Console.WriteLine("Name is :" + model.Name);
                Console.WriteLine("Rating is :" + model.Rating);
                Console.WriteLine("Category name  is :" + model.CategoryName);
                Console.WriteLine("Brand name  is :" + model.BrandName);
                Console.WriteLine("Coulours Selected are : " + model.SelectedColours);
                foreach (var colour in model.SelectedColours)
                {
                    Console.WriteLine(colour);
                }
                Console.WriteLine("Range is : " + model.Price);
                Console.WriteLine("Desciption is : " + model.Description);
                Console.WriteLine("Quantity is : " + model.Quantity);
                Console.WriteLine("Year is : " + model.ModelYear);
                TempData["SuccessMessage"] = "Create post runned";

                var brands = await _UOFInstance._brandsRepository.GetAll();
                var selectedbrand = brands.FirstOrDefault(brd => brd.Name == model.BrandName);

                var category = await _UOFInstance._categoriesRepository.GetAll();
                var selectedCategory = category.FirstOrDefault(cat => cat.Name == model.CategoryName);

                var Colours = await _UOFInstance._coloursRepository.GetAll();
                var selectedclrs = Colours.Where(clr => model.SelectedColours.Contains(clr.Name)).ToList();


                var vehicle = new Vehicles
                {
                    Name = model.Name,
                    Rating = model.Rating,
                    ModelYear = model.ModelYear,
                    Price = model.Price,
                    Description = model.Description,
                    Colours = selectedclrs,
                    Brands = selectedbrand,
                    Categories = selectedCategory
                };

                try
                {
                    await _UOFInstance._vehicleRepository.Insert(vehicle);
                    await _UOFInstance.Save();
                    var stk = new Stocks
                    {
                        Quantity = model.Quantity,
                        VehicleId = vehicle.Id
                    };
                    await _UOFInstance._stocksRepository.Insert(stk);
                    await _UOFInstance.Save();
                    vehicle.StockId = stk.Id;
                    _UOFInstance._vehicleRepository.Update(vehicle);
                    await _UOFInstance.Save();
                    Console.WriteLine("Inserted");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during insert: " + ex.Message);
                    TempData["ErrorMessage"] = "Error during insertion.";
                    return View(model);
                }

                return RedirectToAction("Index", "Vehicles");
            }
            else
            {
                Console.WriteLine("Rating is :" + model.Rating);
                Console.WriteLine("model is null due to rating ");
                model.BrandList = (await _UOFInstance._brandsRepository.GetAll()).Select(brd => brd.Name);
                model.CategoriesList = (await _UOFInstance._categoriesRepository.GetAll()).Select(clg => clg.Name);
                model.ColoursList = (await _UOFInstance._coloursRepository.GetAll()).Select(clr => clr.Name);

                return View(model);
            }

        }

        [HttpGet("Details/{Id}")]
        public async Task<IActionResult> GetVehicles(int id)
        {
            try
            {
                var result = await _UOFInstance._vehicleRepository.GetById(id);

                if (result == null)
                {
                    return NotFound(new { message = "Vehicle not found." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching data", error = ex.Message });
            }
        }

        [HttpGet("Filter")]
        public async Task<IActionResult> Filter(
                string? category,
                string? Brand,
                string? colours,
                int? Rating,
                int? MinPrice,
                int? MaxPrice,
                int PageSize = 10,
                int PageNumber = 1,
                string? SearchTerm = "",
                string? SortColumn = "",
                string? SortDirection = "")
        {
            Console.WriteLine("Detedct the change ");
            Console.WriteLine("valuess are ");
            Console.WriteLine(MinPrice);
            Console.WriteLine(MaxPrice);
            Console.WriteLine("Should not be detected");
            try
            {
                var result = await _UOFInstance._vehicleRepository.GetAll(PageSize, PageNumber, SearchTerm ?? "", SortColumn ?? "Name", SortDirection ?? "ASC", category ?? "", Brand ?? "", MinPrice ?? 200000, MaxPrice ?? 2000000000, "", colours ?? "", Rating ?? 5);
                var categoryList = await _UOFInstance._categoriesRepository.GetAll();
                var BrandsList = await _UOFInstance._brandsRepository.GetAll();
                var ColoursList = await _UOFInstance._coloursRepository.GetAll();
                var data = new
                {
                    result = result,
                    categories = categoryList.Select(cat => cat.Name),
                    Brands = BrandsList.Select(brd => brd.Name),
                    Colours = ColoursList.Select(clr => clr.Name)
                };
                Console.WriteLine("Data sent to frontend: " + JsonConvert.SerializeObject(data));
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching data", error = ex.Message });

            }

        }

    }
}
