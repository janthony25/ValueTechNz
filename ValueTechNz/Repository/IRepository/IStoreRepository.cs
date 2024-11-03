using ValueTechNz.Helpers;
using ValueTechNz.Models.Dto;

namespace ValueTechNz.Repository.IRepository
{
    public interface IStoreRepository
    {
        Task<PaginatedList<GetProductsDto>> GetStoreProductsAsync(int pageNumber, int pageSize, string? searchTerm);
    }
}
