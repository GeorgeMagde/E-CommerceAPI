namespace NoobProject.Dtos.ProductDtos {
    public class ProductQueryParameters {
        public string? SearchName { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
