using System.ComponentModel;

namespace ValueTechNz.Models.Dto
{
    public class GetProductsDto
    {
        public int ProductId { get; set; }

        [DisplayName("Name")]
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }

        [DisplayName("Image")]
        public string? ImageFileName { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; }
        public string CategoryName { get; set; }    
    }
}
