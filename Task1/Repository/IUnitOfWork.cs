using Task1.Models;

namespace Task1.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        //IGenericRepository<Vehicles> _vehicleRepository { get; }
        VehicleRepository _vehicleRepository { get; }
        IGenericRepository<Categories> _categoriesRepository { get; }
        IGenericRepository<Brands> _brandsRepository { get; }
        IGenericRepository<Colours> _coloursRepository { get; }
        IGenericRepository<Stocks> _stocksRepository { get; }
        Task CreateTransaction();
        Task Save();
    }
}
