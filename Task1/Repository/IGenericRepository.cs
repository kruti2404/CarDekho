using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Task1.DTO;
using Task1.ViewModel;

namespace Task1.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        public Task Insert(T entity);
        public void Update(T entity);
        public void Delete(T entity);
        public Task<T> GetById(int id);
        public Task<IEnumerable<T>> GetAll();
        public Task<VehicleViewModel> GetAll(int PageSize, int PageNumber, string SearchTerm, string SortColumn, string SortDirection, string SingleFiltter, string MultiFiltter, int MinPrice, int MaxPrice, string StockAvail, string ColoursSelected, int Rating);



    }
}
