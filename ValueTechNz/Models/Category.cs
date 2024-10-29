using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ValueTechNz.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [DisplayName("Category")]
        public required string CategoryName { get; set; }

        // Many to Many - Product to Category
        public ICollection<ProductCategory> ProductCategory { get; set; }
    }
}
