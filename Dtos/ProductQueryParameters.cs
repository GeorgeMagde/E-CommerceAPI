namespace NoobProject.Dtos {
    public class ProductQueryParameters {
        public string? SearchTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
