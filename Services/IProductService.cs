using NoobProject.Dtos.ProductDtos;

namespace NoobProject.Services {
    public interface IProductService {
        Task<IEnumerable<ProductResponseDto>> GetProductsAsync(ProductQueryParameters queryParameters);
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<ProductResponseDto> CreateProductAsync(CreateUpdateProductDto dto);
    }
}
