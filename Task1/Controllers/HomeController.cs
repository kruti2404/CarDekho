using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task1.Models;
using Task1.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Task1.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IUnitOfWork _UOFInstance;
    private readonly ILogger<HomeController> _logger;
    public HomeController(ILogger<HomeController> logger, IUnitOfWork UOFInstance)
    {
        _UOFInstance = UOFInstance;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public async Task<IActionResult> loadData()
    {

        try
        {
            var categoryList = await _UOFInstance._categoriesRepository.GetAll();
            var BrandsList = await _UOFInstance._brandsRepository.GetAll();
            var ColoursList = await _UOFInstance._coloursRepository.GetAll();

            var data = new
            {
                brandslist = BrandsList.Select(brd => brd.Name),
                categorylist = categoryList.Select(c => c.Name),
                colorList = ColoursList.Select(c => c.Name),
            };

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching data", error = ex.Message });
        }

    }

    [HttpPost]
    public async Task<IActionResult> AddVehicle(Createvehicle formdata)
    {
        Console.WriteLine("Heyyyyy data added successfully");
        Console.WriteLine("Data from the angular app " + formdata.Name);
        Console.WriteLine(formdata.Description);
        Console.WriteLine(formdata.ModalYear);
        Console.WriteLine(formdata.Price);
        Console.WriteLine(formdata.Rating);
        Console.WriteLine(formdata.Quantity);
        Console.WriteLine(formdata.Colours);
        Console.WriteLine(formdata.Brand);
        Console.WriteLine(formdata.Category);
        string[] colours = formdata.Colours[0].Split(",");
        if (formdata.Colours.Any())
        {
            Console.WriteLine(formdata.Colours.Length);
            Console.WriteLine("Colours are ");
            foreach (var colorr in formdata.Colours)
            {

                Console.WriteLine(colorr);

            }

        }
        var brands = await _UOFInstance._brandsRepository.GetAll();
        var selectedbrand = brands.FirstOrDefault(brd => brd.Name == formdata.Brand);

        var category = await _UOFInstance._categoriesRepository.GetAll();
        var selectedCategory = category.FirstOrDefault(cat => cat.Name == formdata.Category);

        var Colours = await _UOFInstance._coloursRepository.GetAll();
        var selectedclrs = Colours.Where(clr => colours.Contains(clr.Name)).ToList();
        foreach (var item in selectedclrs)
        {
            Console.WriteLine(item.Name);

        }

        var vehicle = new Vehicles
        {
            Name = formdata.Name,
            Rating = formdata.Rating,
            ModelYear = formdata.ModalYear,
            Price = formdata.Price,
            Description = formdata.Description,
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
                Quantity = formdata.Quantity,
                VehicleId = vehicle.Id
            };
            await _UOFInstance._stocksRepository.Insert(stk);
            await _UOFInstance.Save();
            vehicle.StockId = stk.Id;
            _UOFInstance._vehicleRepository.Update(vehicle);
            await _UOFInstance.Save();

            Console.WriteLine("Inserted");
            var data = new
            {
                Result = "Successful"
            };
            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error during insert: " + ex.Message);
            return StatusCode(500, new { message = "An error occurred while fetching data", error = ex.Message });

        }
    }

    //Editvehicle
    [HttpPost]
    public async Task<IActionResult> Editvehicle(Createvehicle formdata)
    {
        Console.WriteLine("Heyyyyy data added successfully");
        Console.WriteLine("Data from the angular app " + formdata.Name);
        Console.WriteLine(formdata.Description);
        Console.WriteLine(formdata.ModalYear);
        Console.WriteLine(formdata.Price);
        Console.WriteLine(formdata.Rating);
        Console.WriteLine(formdata.Quantity);
        Console.WriteLine(formdata.Colours);
        Console.WriteLine(formdata.Brand);
        Console.WriteLine(formdata.Category);
        string[] colours = formdata.Colours[0].Split(",");
        if (formdata.Colours.Any())
        {
            Console.WriteLine(formdata.Colours.Length);
            Console.WriteLine("Colours are ");
            foreach (var colorr in formdata.Colours)
            {
                Console.WriteLine(colorr);

            }
        }
        var brands = await _UOFInstance._brandsRepository.GetAll();
        var selectedbrand = brands.FirstOrDefault(brd => brd.Name == formdata.Brand);

        var category = await _UOFInstance._categoriesRepository.GetAll();
        var selectedCategory = category.FirstOrDefault(cat => cat.Name == formdata.Category);

        var Colours = await _UOFInstance._coloursRepository.GetAll();
        var selectedclrs = Colours.Where(clr => colours.Contains(clr.Name)).ToList();
        foreach (var item in selectedclrs)
        {
            Console.WriteLine(item.Name);

        }
        var existingVehicle = (await _UOFInstance._vehicleRepository.GetAll()).FirstOrDefault(veh => veh.Id == Convert.ToInt32(formdata.Id));

        existingVehicle.Name = formdata.Name;
        existingVehicle.ModelYear = formdata.ModalYear;
        existingVehicle.Description = formdata.Description;
        existingVehicle.Price = formdata.Price;
        existingVehicle.Rating = formdata.Rating;
        existingVehicle.BrandID = selectedbrand?.Id;
        existingVehicle.CategoryId = selectedCategory?.Id;
        existingVehicle.Stocks.Quantity = formdata.Quantity;
        existingVehicle.Colours?.Clear();
        existingVehicle.Colours?.Clear();
        foreach (var colour in selectedclrs)
        {
            existingVehicle.Colours?.Add(colour);
        }
        try
        {
            await _UOFInstance.Save();
            Console.WriteLine("The edit is successfull");
            var data = new
            {
                Result = "Successful"
            };
            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error during insert: " + ex.Message);
            return StatusCode(500, new { message = "An error occurred while fetching data", error = ex.Message });

        }
    }


}
