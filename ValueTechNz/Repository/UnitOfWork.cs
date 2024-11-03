using ValueTechNz.Data;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _data;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWebHostEnvironment _environment;
        public UnitOfWork(DataContext data, ILoggerFactory loggerFactory, IWebHostEnvironment environment)
        {
            _data = data;
            _loggerFactory = loggerFactory;
            _environment = environment;
            Products = new ProductsRepository(_data, _loggerFactory, _environment);
            Category = new CategoryRepository(_data, _loggerFactory);
            Store = new StoreRepository(_data, _loggerFactory);
        }
        public IProductsRepository Products { get; private set; }

        public ICategoryRepository Category { get; private set; }

        public IStoreRepository Store { get; private set; }
    }
}
