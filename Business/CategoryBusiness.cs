using ECommerce.Data;
using ECommerce.Dto;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Business
{
    public class CategoryBusiness
    {
        private readonly AppDbContext _context;
        public CategoryBusiness(AppDbContext context)
        {
            _context = context;
        }

        #region Category İslemleri  ------------
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                                 .Include(c => c.Products)
                                 .Include(c => c.SubCategories)
                                    .ThenInclude(sc => sc.Products)
                                 .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                                 .Include(c => c.Products)
                                 .Include(c => c.SubCategories)
                                     .ThenInclude(sc => sc.Products)
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = new Category { Name = dto.Name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }
        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null) return false;

            category.Name= dto.Name;
            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> DeleteCategoryAsync( int id)
        {
            var category = await _context.Categories    
                                         .Include(c => c.SubCategories)   
                                              .ThenInclude(sc=> sc.Products)
                                         .FirstOrDefaultAsync(c => c.Id==id);

            if (category is null) return false;

            foreach(var sub in category.SubCategories)
            {
                _context.Products.RemoveRange(sub.Products);
            }
             
            _context.SubCategories.RemoveRange(category.SubCategories);
            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return true;
        }

        #endregion

        #region SubCategory İslemleri --------------
        public async Task<List<SubCategory>> GetSubCategoriesAsync(int categoryId)
        {
            return await _context.SubCategories
                                   .Where(sc=> sc.CategoryId==categoryId)
                                   .Include(sc=> sc.Products)
                                   .ToListAsync();
        }

        public async Task<SubCategory?> AddSubCategoryAsync(int categoryId, CreateSubCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(categoryId);

            if (category is null) return null;

            var subCategory = new SubCategory
            { 
                Name = dto.Name, 
                CategoryId = categoryId 
            };
            _context.SubCategories.Add(subCategory);
            await _context.SaveChangesAsync();
            return subCategory;
        }

        public async Task<bool> UpdateSubCategoryAsync(int id, UpdateSubCategoryDto dto)
        {
            var sub = await _context.SubCategories.FindAsync(id);
            if (sub is null) return false;

            bool categoryExists = await _context.Categories.AnyAsync(c=>c.Id==dto.CategoryId);
            if (!categoryExists) return false;

            sub.Name= dto.Name;
            sub.CategoryId= dto.CategoryId;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteSubCategoryAsync(int id)
        {
            var subCategory= await _context.SubCategories
                                           .Include(sc=>sc.Products)
                                           .FirstOrDefaultAsync(sc=> sc.Id==id);
            if(subCategory is null) return false;

            _context.Products.RemoveRange(subCategory.Products);
            _context.SubCategories.Remove(subCategory);
            await _context.SaveChangesAsync();

            return true;
        }

        #endregion
        public async Task<List<Product>> GetProductsBySubCategoryAsync(int subCategoryId)
        {
            return await _context.Products
                                 .Where(p=>p.SubCategoryId==subCategoryId)
                                 .Include(p=>p.SubCategory)
                                     .ThenInclude(sc=>sc.Category)
                                 .ToListAsync();
        }
    }
}
