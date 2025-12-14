using ECommerce.Data;
using ECommerce.Dto;
using ECommerce.Helpers;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using System.Security.Claims;

namespace ECommerce.Services
{
    public class OrderServices
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor; // giriş yapan kullanıcıya ulaşmak için 

        public OrderServices(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> CreateOrderAsync(CreateOrderDto orderDto)
        {
            var userIdString = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier); //tokendan giriş yapan kullanıcı idsi
            if (userIdString == null)
            {
                return "Kullanıcı Doğrulanamadı";
            }
            int userId = int.Parse(userIdString);
            decimal totalPrice = 0; // toplam sipariş tutarı
            var orderItems = new List<OrderItem>();// siparişteki üürnleri tutan liste

            foreach (var item in orderDto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId); // veritabanından ürünü bul
                if (product == null)
                {
                    return $"Ürün Bulunamadı. ID: {item.ProductId}";
                }
                totalPrice += (decimal)(product.Price * item.Quantity);

                orderItems.Add(new OrderItem // siparişi oluştur ve listeye ekle
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = (decimal)product.Price,
                });
            }

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                TotalPrice = totalPrice,
                Items = orderItems
            };

            _context.Orders.Add(order);//siparişi ekleyip kaydet
            await _context.SaveChangesAsync();

            return "Sipariş başarıyla oluşturuldu.";

        }

        public async Task<ServiceResult> GetAllOrdersAsync()
        {
            var orders=  await _context.Orders // siparişleri ve içindeki orderitemları ve onlara ait productları birlikte yükle
                .Include(o => o.Items)//siparişini çindeki ürünleri de getir
                .ThenInclude(i => i.Product) // ürün bilgilerini de dahil et
                .ToListAsync(); // listele

            return new ServiceResult
            {
                Success = true,
                Message = "Tüm Siparişler Başarıyla Getirildi",
                Data = orders
            };
        }

        public async Task<ServiceResult> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders 
                .Where(o => o.Id == id)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product) 
                .FirstOrDefaultAsync();// sonuç varsa getir yoksa null

            if (order == null)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = "Sipariş Bulunamadı"
                };
            }

            return new ServiceResult
            {
                Success = true,
                Message = "Sipariş Başarıyla Getirildi",
                Data = order
            };
        }

        public async Task<ServiceResult> UpdateOrderAsync(int id, CreateOrderDto dto)
        {
            var exitingOrder = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o=> o.Id==id);
            if (exitingOrder == null)
                return new ServiceResult
                {
                    Success = false,
                    Message = "Sipariş Bulunamadı"
                };

            _context.OrderItems.RemoveRange(exitingOrder.Items);

            exitingOrder.Items = dto.Items.Select(i=> new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
            }).ToList();

            await _context.SaveChangesAsync();
            return new ServiceResult
            {
                Success = true,
                Message = "Sipariş Başarıyla Güncellendi"
            };
        }

        public async Task<ServiceResult> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id); // siparişi ilişkili ürünleriyle bul

            if (order == null)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = "Sipariş Bulunamadı"
                };
            }
        

            _context.Orders.Remove(order); //siparişi sil
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                Success = true,
                Message = "Sipariş Başarıyla Silindi"
            };
                 
        }
    }
}
