namespace ECommerce.Dto
{
    public class CreateOrderDto
    {
        public List<OrderItemDto> Items { get; set; } // Siparişteki ürünler
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }  // Ürün ID'si
        public int Quantity { get; set; }   // Kaç adet alınmış
    }
}
