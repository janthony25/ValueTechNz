using ValueTechNz.Models.Dto;

namespace ValueTechNz.Repository.IRepository
{
    public interface IProductsRepository
    {
        Task<List<GetProductsDto>> GetAllProductsAsync();
        Task AddProductAsync(AddUpdateProductDto addProductDto);
        Task<AddUpdateProductDto> GetProductByIdAsync(int id);
        Task UpdateProductAsync(int id, AddUpdateProductDto updateProductDto);
    }
}
