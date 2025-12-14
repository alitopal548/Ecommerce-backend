using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string? ProductName { get; set; }

        [MaxLength(500)]
        public string? ProductDescription { get; set; }

        [Required]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public SubCategory? SubCategory { get; set; }

        public List<ProductImages> ProductImages { get; set; } = new List<ProductImages>();
    }
}
