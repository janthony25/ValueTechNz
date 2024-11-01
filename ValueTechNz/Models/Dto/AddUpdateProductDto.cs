using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ValueTechNz.Models.Dto
{
    public class AddUpdateProductDto
    {
        public int ProductId { get; set; }

        [DisplayName("Name")]
        public required string ProductName { get; set; }
        public required string Brand { get; set; }
        public required decimal Price { get; set; }
        public string? Description { get; set; }

        [DisplayName("Image")]
        public IFormFile? ImageFile { get; set; }
        public DateTime? DateUpdated { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        public required int CategoryId { get; set; }

        [DisplayName("Category")]
        public string? CategoryName { get; set; } 
    }
}
