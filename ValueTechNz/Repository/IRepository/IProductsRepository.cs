using ValueTechNz.Helpers;
using ValueTechNz.Models.Dto;

namespace ValueTechNz.Repository.IRepository
{
    public interface IProductsRepository
    {
        Task<PaginatedList<GetProductsDto>> GetPaginatedProductsAsync(int pageNumber,
                                                                      int pageSize,
                                                                      string? searchTerm,
                                                                      string sortColumn = "DateAdded",
                                                                      string sortOrder = "desc");
        Task AddProductAsync(AddUpdateProductDto addProductDto);
        Task<AddUpdateProductDto> GetProductByIdAsync(int id);
        Task UpdateProductAsync(int id, AddUpdateProductDto updateProductDto);
        Task DeleteProductAsync(int id);
        Task<List<GetProductsDto>> GetLatestProductsAsync();
    }
}
