using ValueTechNz.Data;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _data;
        private readonly ILoggerFactory _loggerFactory;
        public UnitOfWork(DataContext data, ILoggerFactory loggerFactory)
        {
            _data = data;
            _loggerFactory = loggerFactory;
            Products = new ProductsRepository(_data, _loggerFactory);
        }
        public IProductsRepository Products { get; private set; }
    }
}
