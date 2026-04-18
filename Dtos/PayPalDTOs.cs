using System.Text.Json.Serialization;

namespace NoobProject.Dtos
{
    public class PayPalOrderRequest
    {
        [JsonPropertyName("intent")]
        public string Intent { get; set; } = "CAPTURE";

        [JsonPropertyName("purchase_units")]
        public List<PurchaseUnit> PurchaseUnits { get; set; } = new();

        [JsonPropertyName("application_context")]
        public ApplicationContext ApplicationContext { get; set; } = new();
    }

    public class PurchaseUnit
    {
        [JsonPropertyName("amount")]
        public Amount Amount { get; set; } = new();

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

    public class Amount
    {
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; } = "USD";

        [JsonPropertyName("value")]
        public string Value { get; set; } = "0.00";
    }

    public class ApplicationContext
    {
        [JsonPropertyName("return_url")]
        public string ReturnUrl { get; set; } = string.Empty;

        [JsonPropertyName("cancel_url")]
        public string CancelUrl { get; set; } = string.Empty;

        [JsonPropertyName("shipping_preference")]
        public string ShippingPreference { get; set; } = "NO_SHIPPING";
    }

    public class PayPalOrderResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("links")]
        public List<Link> Links { get; set; } = new();
    }

    public class Link
    {
        [JsonPropertyName("href")]
        public string Href { get; set; } = string.Empty;

        [JsonPropertyName("rel")]
        public string Rel { get; set; } = string.Empty;

        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;
    }

    public class PayPalAccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
