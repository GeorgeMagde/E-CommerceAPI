using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using NoobProject.Contexts;
using NoobProject.Dtos.ProductDtos;
using NoobProject.Helper;
using NoobProject.Models;

namespace NoobProject.Services {
    public class ProductService : IProductService {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly ImageHelper imageHelper;

        public ProductService(AppDbContext _context, IWebHostEnvironment _environment, ImageHelper imageHelper) {
            context = _context;
            environment = _environment;
            this.imageHelper = imageHelper;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsAsync(ProductQueryParameters queryParams, string requestScheme, string requestHost) {
            var query = context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.SearchName))
                query = query.Where(p => p.Name.Contains(queryParams.SearchName));

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
                Image = !string.IsNullOrEmpty(p.Image) ? imageHelper.ResolveImageUrl(p.Image, requestScheme, requestHost) : null
            });
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id, string requestScheme, string requestHost) {
            var p = await context.Products.FindAsync(id);
            if (p == null) return null;

            return new ProductResponseDto {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Image = !string.IsNullOrEmpty(p.Image) ? imageHelper.ResolveImageUrl(p.Image, requestScheme, requestHost) : null
            };
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateUpdateProductDto dto, string requestScheme, string requestHost) {
            string imagePath = null;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0) {
                imagePath = await imageHelper.SaveImageAsync(dto.ImageFile);
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
                Image = !string.IsNullOrEmpty(imagePath)? imageHelper.ResolveImageUrl(imagePath, requestScheme, requestHost): null
            };
        }
    }
}
