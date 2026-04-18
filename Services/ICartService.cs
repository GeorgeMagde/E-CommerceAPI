using NoobProject.Dtos.CartDtos;
using NoobProject.Models;

namespace NoobProject.Services
{
    public interface ICartService
    {
        Task<Cart> GetOrCreateCart(string userId);
        Task<CartDto> GetCartAsync(string userId);
        Task AddToCartAsync(string userId, int productId, int quantity);
        Task UpdateQuantityAsync(string userId, int productId, int quantity);
        Task<bool> RemoveFromCartAsync(string userId, int productId);
        Task ClearCartAsync(string userId);

    }
}
