using System.ComponentModel.DataAnnotations;

namespace ECommerce.Dto
{
    public class UpdateProductDto
    {
        [Required]
        public string ProductName { get; set; }

        public string? ProductDescription { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int SubCategoryId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<IFormFile>? Images { get; set; }
    }
}
