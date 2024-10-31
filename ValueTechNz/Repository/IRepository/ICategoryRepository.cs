using ValueTechNz.Models.Dto;

namespace ValueTechNz.Repository.IRepository
{
    public interface ICategoryRepository
    {
        Task<List<CategoryListDto>> GetCategoryListAsync();
    }
}
