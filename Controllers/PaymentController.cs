using Microsoft.AspNetCore.Mvc;
using NoobProject.Services;

namespace NoobProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPayPalService _payPalService;

        public PaymentController(IPayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromQuery] decimal amount)
        {
            try
            {
                
                string returnUrl = "https://localhost:4200/success";
                string cancelUrl = "https://localhost:4200/cancel";

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
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
