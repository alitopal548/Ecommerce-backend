using ECommerce.Models;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products {  get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<RegistrationRequest> RegistrationRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Order - OrderItem 1 e çok
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)// bir sipaerişin biren çok orderitemi olabilir
                .WithOne(oi => oi.Order)// her orderitem bir siparişe aittir
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem - Product 
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            // Order - User 
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);

        }


    }
}
