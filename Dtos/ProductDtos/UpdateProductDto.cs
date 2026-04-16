using System.ComponentModel.DataAnnotations;

namespace NoobProject.Dtos.ProductDtos {
    public class UpdateProductDto {
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters.")]
        public string? Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Range(0.01, 100000, ErrorMessage = "Price must be between 0.01 and 100000.")]
        public decimal? Price { get; set; }

        [Range(0, 10000, ErrorMessage = "Stock cannot be negative.")]
        public int? Stock { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
