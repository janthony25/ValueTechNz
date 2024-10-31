using System.ComponentModel;

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
        
        public required int CategoryId { get; set; }

        [DisplayName("Category")]
        public required string CategoryName { get; set; } 
    }
}
