using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Bu kategoriye ait ürünler
        public List<Product> Products { get; set; } = new List<Product>();

        // Bu kategoriye ait alt kategoriler (ayrı tablodan)
        public List<SubCategory> SubCategories { get; set; } = new();
    }
}
