using NoobProject.Dtos.ProductDtos;

namespace NoobProject.Services {
    public interface IProductService {
        Task<IEnumerable<ProductResponseDto>> GetProductsAsync(ProductQueryParameters queryParameters, string requestScheme, string requestHost);
        Task<ProductResponseDto?> GetProductByIdAsync(int id, string requestScheme, string requestHost);
        Task<ProductResponseDto> CreateProductAsync(CreateUpdateProductDto dto, string requestScheme, string requestHost);
        Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto dto, string requestScheme, string requestHost);
        Task<bool> DeleteProductAsync(int id);
    }
}
