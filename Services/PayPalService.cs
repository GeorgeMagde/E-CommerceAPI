using Microsoft.Extensions.Options;
using NoobProject.Dtos;
using NoobProject.Helper;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NoobProject.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly PayPalSettings _settings;

        public PayPalService(HttpClient httpClient, IOptions<PayPalSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> GetAccessToken()
        {
            var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.Secret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v1/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authString);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<PayPalAccessTokenResponse>(content);

            return tokenResponse?.AccessToken ?? string.Empty;
        }

        public async Task<PayPalOrderResponse> CreateOrder(decimal amount, string returnUrl, string cancelUrl)
        {
            var accessToken = await GetAccessToken();

            var orderRequest = new PayPalOrderRequest
            {
                Intent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnit>
                {
                    new PurchaseUnit
                    {
                        Amount = new Amount
                        {
                            CurrencyCode = "USD",
                            Value = amount.ToString("F2")
                        },
                        Description = "E-Commerce Order"
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    ShippingPreference = "NO_SHIPPING"
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v2/checkout/orders");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(JsonSerializer.Serialize(orderRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PayPalOrderResponse>(content) ?? new PayPalOrderResponse();
        }

        public async Task<PayPalOrderResponse> CaptureOrder(string orderId)
        {
            var accessToken = await GetAccessToken();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v2/checkout/orders/{orderId}/capture");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"PayPal Capture Error: {response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PayPalOrderResponse>(content) ?? new PayPalOrderResponse();
        }
    }
}
