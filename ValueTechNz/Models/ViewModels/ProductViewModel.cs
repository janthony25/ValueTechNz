using ValueTechNz.Models.Dto;

namespace ValueTechNz.Models.ViewModels
{
    public class ProductViewModel
    {
        public AddUpdateProductDto Product { get; set; }
        public IEnumerable<CategoryListDto> CategoryList { get; set; }      
    }
}
