using NoobProject.Dtos;

namespace NoobProject.Services
{
    public interface IPayPalService
    {
        Task<string> GetAccessToken();
        Task<PayPalOrderResponse> CreateOrder(decimal amount, string returnUrl, string cancelUrl);
        Task<PayPalOrderResponse> CaptureOrder(string orderId);
    }
}
