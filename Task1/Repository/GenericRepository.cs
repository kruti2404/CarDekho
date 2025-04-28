using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Task1.Data;
using Task1.DTO;
using Task1.ViewModel;


namespace Task1.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> _table;
        private readonly ProgramDbContext _context;

        public GenericRepository(ProgramDbContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        //public async Task<T> GetById(int id)
        //{
        //    return await _table.FindAsync(id);

        //}
        virtual
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _table.ToListAsync();
        }

        public async Task<VehicleDTO> GetById(int Id)
        {
            var result = await _context.Database.SqlQueryRaw<VehicleDTO>("Exec GetRecordById @Id",
                                                                            new SqlParameter("@Id", Id)).ToListAsync();
            var singlerecord = result.FirstOrDefault(rst => rst.Id == Id);

            return singlerecord ?? new VehicleDTO();

        }

        public async Task<VehicleViewModel> GetAll(int PageSize, int PageNumber, string SearchTerm, string SortColumn, string SortDirection, string SingleFiltter, string MultiFiltter, int MinPrice, int MaxPrice, string StockAvail, string ColoursSelected, int Rating)
        {
            bool? StockAvailBool;
            if (StockAvail == "OutOfStock")
            {
                StockAvailBool = false;
            }
            else if (StockAvail == "InStock")
            {
                StockAvailBool = true;
            }
            else
            {
                StockAvailBool = null;
            }
            SqlParameter TotalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int)
            {
                ParameterName = "@TotalRecords",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output,
                Size = sizeof(int)
            };


            var result = await _context.Database.SqlQueryRaw<VehicleDTO>("Exec SearchVehicles @PageSize, @PageNumber, @SearchTerm, @SortColumn, @SortDirection, @SingleFiltter, @MultiFiltter, @MinPrice, @MaxPrice, @StockAvail, @ColoursList, @Rating, @TotalRecords OUTPUT",
                                                                         new SqlParameter("@PageSize", PageSize),
                                                                         new SqlParameter("@PageNumber", PageNumber),
                                                                         new SqlParameter("@SearchTerm", SearchTerm),
                                                                         new SqlParameter("@SortColumn", SortColumn),
                                                                         new SqlParameter("@SortDirection", SortDirection),
                                                                         new SqlParameter("@SingleFiltter", SingleFiltter),
                                                                         new SqlParameter("@MultiFiltter", MultiFiltter),
                                                                         new SqlParameter("@MinPrice", MinPrice),
                                                                         new SqlParameter("@MaxPrice", MaxPrice),
                                                                         new SqlParameter("@ColoursList", ColoursSelected),
                                                                         new SqlParameter("@StockAvail", (object)StockAvailBool ?? DBNull.Value),
                                                                         new SqlParameter("@Rating", Rating),

                                                                         TotalRecordsParam).ToListAsync();



            int totalRecords = TotalRecordsParam.Value != DBNull.Value ? (int)TotalRecordsParam.Value : 0;

            var vehicleModel = new VehicleViewModel
            {
                Vehicles = result,
                TotalPages = totalRecords,
            };
            Console.WriteLine("Total raw :" + totalRecords);

            return (vehicleModel);
        }

        public async Task Insert(T entity)
        {
            await _table.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _table.Update(entity);
        }

        public void Delete(T entity)
        {
            _table.Remove(entity);
        }


    }
}
