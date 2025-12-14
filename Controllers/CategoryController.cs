using ECommerce.Business;
using ECommerce.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryBusiness _business;

        public CategoryController(CategoryBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var list = await _business.GetAllCategoriesAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _business.GetCategoryByIdAsync(id);

            if (category is null) return NotFound("İlgili Kategoriye ait sonuç bulunamadı");

            return Ok(category);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Kategori adı boş olamaz.");

            var created = await _business.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = created.Id }, created);
        }


        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Kategori adı boş olamaz.");

            bool category = await _business.UpdateCategoryAsync(id, dto);

            if (!category) return NotFound("Kategori bulunamadı.");

            return Ok("Kategori güncellendi.");
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            bool category = await _business.DeleteCategoryAsync(id);
            if (!category) return NotFound("İlgili Kategori Bulunamadı");

            return Ok("Kategori ve ilişkili tüm alt kategori ve ürünler silindi.");
        }


        [HttpGet("{categoryId}/subcategories")]
        public async Task<IActionResult> GetSubCategories(int categoryId)
        {
            var subCategoryList = await _business.GetSubCategoriesAsync(categoryId);

            return Ok(subCategoryList);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("{categoryId}/subcategories")]
        public async Task<IActionResult> AddSubCategory(int categoryId, [FromBody] CreateSubCategoryDto dto)
        {
            var subCategory = await _business.AddSubCategoryAsync(categoryId, dto);
            if (subCategory is null) return NotFound("Ana kategori bulunamadı.");

            return Ok(subCategory);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("subcategories/{id}")]
        public async Task<IActionResult> UpdateSubCategory(int id, [FromBody] UpdateSubCategoryDto dto)
        {
           bool sub = await _business.UpdateSubCategoryAsync(id, dto);
            if (!sub) return NotFound("Alt Kategori veya Ana Kategori bulunamadı.");

            return Ok("Alt kategori güncellendi.");
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("subcategories/{id}")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            bool subCategory = await _business.DeleteSubCategoryAsync(id);

            if (!subCategory) return NotFound("Alt kategori bulunamadı.");

            return Ok("Alt kategori ve ilişkili ürünler silindi.");
        }

        [HttpGet("by-subcategory/{subCategoryId}")]
        public async Task<IActionResult> GetProductsBySubCategory(int subCategoryId)
        {
            var productsList = await _business.GetProductsBySubCategoryAsync(subCategoryId);
            return Ok(productsList);
        }

    }
}