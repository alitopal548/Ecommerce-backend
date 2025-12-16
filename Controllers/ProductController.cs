using ECommerce.Business;
using ECommerce.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductBusiness _business;
        public ProductController(ProductBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public ActionResult GetAllProducts() 
        {
            var product = _business.GetAllProducts();
            return Ok(product);
        }

        [HttpGet("{id}")]
        public ActionResult GetProductById(int id)
        {
            var product = _business.GetProductById(id);
            if (product == null)
                return NotFound("ürün bulunamadı");
            
            return Ok(product);
        }

        [Authorize(Roles = "admin")] // sadece admin erişebilir, ürün ekle
        [HttpPost]
        public async Task<IActionResult> ProductCreate([FromForm] CreateProductDto dto)
        {
            var createdProduct = await _business.CreateProductAsync(dto);
            if (createdProduct == null)
                return BadRequest("Ürün oluşturulamadı. Lütfen alanları kontrol ediniz.");

            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ProductUpdate(int id, [FromForm] UpdateProductDto dto)
        {
            bool updatedProduct = await _business.UpdateProductAsync(id, dto);

            if (!updatedProduct)
                return NotFound("Güncellenecek ürün bulunamadı");

            return Ok("Ürün başarıyla güncellendi");
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deletedProduct = await _business.DeleteProductAsync(id);

            if (!deletedProduct)
                return NotFound("Silinecek ürün bulunamadı.");

            return Ok("Ürün başarıyla silindi.");
        }

        [Authorize(Roles ="admin")]
        [HttpDelete("DeleteImage/{imageId}")]
        public async Task<IActionResult> DeleteProductImage(int imageId)
        {
            var deletedImage = await _business.DeleteProductImageAsync(imageId);

            if (!deletedImage)
                return NotFound("Resim bulunamadı");

            return Ok("Resim başarıyla silindi");
        }
    }
}
