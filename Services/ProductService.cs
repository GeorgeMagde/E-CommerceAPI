using Microsoft.EntityFrameworkCore;
using NoobProject.Contexts;
using NoobProject.Dtos;
using NoobProject.Models;

namespace NoobProject.Services {
    public class ProductService : IProductService {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductService(AppDbContext _context, IWebHostEnvironment _environment) {
            context = _context;
            environment = _environment;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsAsync(ProductQueryParameters queryParams) {
            var query = context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
                query = query.Where(p => p.Name.Contains(queryParams.SearchTerm));

            if (queryParams.MinPrice.HasValue)
                query = query.Where(p => p.Price >= queryParams.MinPrice.Value);

            if (queryParams.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= queryParams.MaxPrice.Value);

            var products = await query.ToListAsync();

            return products.Select(p => new ProductResponseDto {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Image = p.Image
            });
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id) {
            var p = await context.Products.FindAsync(id);
            if (p == null) return null;

            return new ProductResponseDto {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Image = p.Image
            };
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateUpdateProductDto dto) {
            string imagePath = string.Empty;

           
            if (dto.ImageFile != null && dto.ImageFile.Length > 0) {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var uploadsFolder = Path.Combine(environment.WebRootPath, "images");

                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }

                imagePath = $"/images/{fileName}";
            }

            var product = new Product {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                Image = imagePath
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            return new ProductResponseDto {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Image = product.Image
            };
        }
    }
}
