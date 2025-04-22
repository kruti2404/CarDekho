using Microsoft.EntityFrameworkCore;
using Task1.Data;
using Task1.DTO;
using Task1.Models;

namespace Task1.Repository
{
    public class VehicleRepository : GenericRepository<Vehicles>
    {
        private readonly ProgramDbContext _context;
        public VehicleRepository(ProgramDbContext context) : base(context)
        {
            _context = context;
        }
        override
        public async Task<IEnumerable<Vehicles>> GetAll()
        {
            var result = await _context.Vehicles.Include(veh => veh.Colours)
                                        .Include(veh => veh.Brands)
                                        .Include(veh => veh.Categories)
                                        .Include(veh => veh.Stocks)
                                        .ToListAsync();

            return result;
        }
        public async Task<VehicleForm> GetByIdIncludingAll(int id)
        {
            var result = await _context.Vehicles.Include(veh => veh.Brands)
                                           .Include(veh => veh.Categories)
                                           .Include(veh => veh.Colours)
                                           .Include(veh => veh.Stocks)
                                           .FirstOrDefaultAsync(veh => veh.Id == id);

            if (result == null)
            {
                return null;
            }

            var selectedColours = result.Colours != null && result.Colours.Any()
                    ? result.Colours.Select(c => c.Name).ToList()
                    : new List<string>();

            var vehForm = new VehicleForm
            {
                VehicleId = id,
                Name = result.Name,
                ModelYear = result.ModelYear,
                Description = result.Description,
                Price = result.Price,
                Rating = result.Rating,
                BrandName = result.Brands?.Name,
                CategoryName = result.Categories?.Name,
                Quantity = result.Stocks.Quantity,
                SelectedColours = string.Join(",", selectedColours)
            };

            return vehForm;
        }

    }
}
