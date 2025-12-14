using ECommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Dto;

namespace ECommerce.Controllers
{
    [Authorize]
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderServices _orderServices;
        public OrderController(OrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        [HttpPost] // YENİ SİPARİŞ OLUŞTUR
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            var result = await _orderServices.CreateOrderAsync(orderDto);

            if (result == "Sipariş başarıyla oluşturuldu.")
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderServices.GetAllOrdersAsync();
            
            if(!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await _orderServices.GetOrderByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] CreateOrderDto dto)
        {
            var result = await _orderServices.UpdateOrderAsync(id, dto);

            if (result.Success)
                return Ok(result);

            return NotFound(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result= await _orderServices.DeleteOrderAsync(id);

            if(result.Success)
                return Ok(result);

            return NotFound(result);
        }
    }
}
