using System.ComponentModel.DataAnnotations;

namespace NoobProject.Dtos {
    public class CreateUpdateProductDto {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100000, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
