using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ValueTechNz.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }      

        [DisplayName("Name")]
        public required string ProductName { get; set; }
        public required string Brand { get; set; }
        public required decimal Price { get; set; }
        public string? Description { get; set; }

        [DisplayName("Image")]
        public string? ImageFileName { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; }

        // Many to Many - Product to Category
        public ICollection<ProductCategory> ProductCategory { get; set; }       
    }
}
