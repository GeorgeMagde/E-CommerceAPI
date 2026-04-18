using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoobProject.Dtos.CartDtos;
using NoobProject.Services;
using System.Security.Claims;

namespace NoobProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartRequestDto dto)
        {
            if (dto.Quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            var userId = GetUserId();
            await _cartService.AddToCartAsync(userId, dto.ProductId, dto.Quantity);
            return Ok(new { Message = "Product added to cart successfully." });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity([FromBody] CartRequestDto dto)
        {
            if (dto.Quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            var userId = GetUserId();
            await _cartService.UpdateQuantityAsync(userId, dto.ProductId, dto.Quantity);
            return Ok(new { Message = "Cart updated successfully." });
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = GetUserId();
            var result = await _cartService.RemoveFromCartAsync(userId, productId);

            if (!result)
                return NotFound("Product not found in cart.");

            return Ok(new { Message = "Product removed from cart successfully." });
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            await _cartService.ClearCartAsync(userId);
            return Ok(new { Message = "Cart cleared successfully." });
        }
    }
}
