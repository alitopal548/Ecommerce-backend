using ECommerce.Data;
using ECommerce.Dto;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Business
{
    public class ProductBusiness
    {
        private readonly AppDbContext _context;
        private readonly string _uploadPath;

        public ProductBusiness(AppDbContext context)
        {
            _context = context;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.SubCategory)
                .Include(p => p.ProductImages)
                .ToList();
        }

        public Product? GetProductById(int id)
        {
            return _context.Products
                .Include(p => p.SubCategory)
                    .ThenInclude(sc => sc.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == id);
        }

        public async Task<Product?> CreateProductAsync(CreateProductDto dto)
        {
            if(string.IsNullOrWhiteSpace(dto.ProductName))
                return null;

            var product = new Product
            {
                ProductName = dto.ProductName,
                ProductDescription = dto.ProductDescription,
                Price = dto.Price,
                SubCategoryId = dto.SubCategoryId,
                CreatedAt = DateTime.Now
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();


            if (dto.Images != null && dto.Images.Length > 0)
            {
                foreach (var image in dto.Images)
                {
                    var fileName =$"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    var filePath = Path.Combine(_uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var productImage = new ProductImages
                    {
                        ProductId = product.Id,
                        ImageUrl = "/uploads/" + fileName
                    };
                    _context.ProductImages.Add(productImage);
                }

                await _context.SaveChangesAsync();
            }

            var created = await _context.Products
                .Include(p => p.SubCategory)
                    .ThenInclude(sc => sc.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            return created;
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync (p => p.Id == id);

            if(product == null) 
                return false;

            product.ProductName = dto.ProductName;
            product.ProductDescription = dto.ProductDescription;
            product.Price = dto.Price;
            product.SubCategoryId = dto.SubCategoryId;
            product.CreatedAt = dto.CreatedAt;

            if (dto.Images != null && dto.Images.Any())
            {
                foreach (var image in dto.Images)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    var filePath = Path.Combine(_uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var productImage = new ProductImages
                    {
                        ProductId = product.Id,
                        ImageUrl = "/uploads/" + fileName
                    };
                    _context.ProductImages.Add(productImage);
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> DeleteProductImageAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null) return false;

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
