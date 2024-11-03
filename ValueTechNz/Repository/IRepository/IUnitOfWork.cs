namespace ValueTechNz.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductsRepository Products { get; }
        ICategoryRepository Category { get; }
        IStoreRepository Store { get; }
    }
}
