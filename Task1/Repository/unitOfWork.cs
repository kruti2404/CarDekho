
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Task1.Data;
using Task1.Models;

namespace Task1.Repository
{
    public class unitOfWork : IUnitOfWork
    {
        public IGenericRepository<Vehicles> _vehicleRepository { get; }
        public IGenericRepository<Categories> _categoriesRepository { get; }
        public IGenericRepository<Brands> _brandsRepository { get; }
        bool isDisposed;
        private readonly ProgramDbContext _context;

        public IDbContextTransaction _TransactionObj = null;

        public unitOfWork(ProgramDbContext context)
        {
            _context = context;
            _vehicleRepository = new GenericRepository<Vehicles>(_context);
            _categoriesRepository = new GenericRepository<Categories>(_context);
            _brandsRepository = new GenericRepository<Brands>(_context);
        }

        public async Task CreateTransaction()
        {
            _TransactionObj = await _context.Database.BeginTransactionAsync();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }


        public void Dispose()
        {
            _context.Dispose();
        }

        public void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                    GC.SuppressFinalize(this);
                }
                isDisposed = true;
            }

        }

    }
}
