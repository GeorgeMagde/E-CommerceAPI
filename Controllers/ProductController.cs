using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoobProject.Dtos.ProductDtos;
using NoobProject.Services;

namespace NoobProject.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase {
        private readonly IProductService productService;

        public ProductController(IProductService productService) {
            this.productService = productService;
        }

        // GET: api/product?searchTerm=laptop&minPrice=500&maxPrice=1500
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParameters queryParams) {
            var products = await productService.GetProductsAsync(queryParams);
            return Ok(products);
        }

        // GET: api/product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id) {
            var product = await productService.GetProductByIdAsync(id);
            if (product == null) return NotFound("Product not found.");

            return Ok(product);
        }

        // POST: api/product 
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateUpdateProductDto dto) {
            // Note: [ApiController] automatically returns 400 Bad Request if the DTO validation fails.
            var createdProduct = await productService.CreateProductAsync(dto);

            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }
    }
}
