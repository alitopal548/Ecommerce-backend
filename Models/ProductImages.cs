using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class ProductImages
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
