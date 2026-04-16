namespace NoobProject.Dtos.ProductDtos {
    public class ProductResponseDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Image { get; set; }

        public bool IsAvailable => Stock > 0;
    }
}
