using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoobProject.Contexts;
using NoobProject.Services;
using System.Security.Claims;

namespace NoobProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPayPalService _payPalService;
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;

        public PaymentController(IPayPalService payPalService, AppDbContext context, ICartService cartService)
        {
            _payPalService = payPalService;
            _context = context;
            _cartService = cartService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromQuery] decimal amount)
        {
            try
            {
                string returnUrl = "http://localhost:4200/success";
                string cancelUrl = "http://localhost:4200/cancel";

                var response = await _payPalService.CreateOrder(amount, returnUrl, cancelUrl);
                
                var approvalLink = response.Links.FirstOrDefault(x => x.Rel == "approve")?.Href;

                return Ok(new
                {
                    OrderId = response.Id,
                    Status = response.Status,
                    ApprovalLink = approvalLink
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("capture/{orderId}")]
        public async Task<IActionResult> CapturePayment(string orderId)
        {
            try
            {
                var response = await _payPalService.CaptureOrder(orderId);

                if (response.Status == "COMPLETED")
                {
                    var userId = GetUserId();
                    var cart = await _context.Carts
                        .Include(c => c.Items)
                        .FirstOrDefaultAsync(c => c.UserId == userId);

                    if (cart != null && cart.Items.Any())
                    {
                        foreach (var entry in cart.Items)
                        {
                            var product = await _context.Products.FindAsync(entry.ProductId);
                            if (product != null)
                            {
                                product.Stock -= entry.Quantity;
                            }
                        }

                        // Clear the cart (ClearCartAsync saves changes, but we've also modified products in this context)
                        await _cartService.ClearCartAsync(userId);
                    }
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
