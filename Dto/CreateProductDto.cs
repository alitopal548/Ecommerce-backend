namespace ECommerce.Dto
{
    public class CreateProductDto
    {
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal Price { get; set; }
        public int SubCategoryId { get; set; }
        public IFormFile[] Images { get; set; }
    }
}
