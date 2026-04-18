using Microsoft.EntityFrameworkCore;
using NoobProject.Contexts;
using NoobProject.Dtos;
using NoobProject.Dtos.CartDtos;
using NoobProject.Models;

namespace NoobProject.Services
{
    public class CartService:ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetOrCreateCart(string userId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }
        public async Task<CartDto> GetCartAsync(string userId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return new CartDto();

            var items = await (
                from ci in _context.CartEntries
                join p in _context.Products
                    on ci.ProductId equals p.Id
                where ci.CartId == cart.Id
                select new CartItemDto
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = ci.Quantity,
                    Total = p.Price * ci.Quantity
                }
            ).ToListAsync();

            return new CartDto
            {
                CartId = cart.Id,
                Items = items,
                TotalPrice = items.Sum(x => x.Total)
            };
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than 0");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new Exception("Product not found");

            if (product.Stock < quantity)
                throw new Exception("Not enough stock");

            var cart = await GetOrCreateCart(userId);

            var item = await _context.CartEntries
                .FirstOrDefaultAsync(i =>
                    i.CartId == cart.Id &&
                    i.ProductId == productId);

            if (item != null)
            {
                if (product.Stock < item.Quantity + quantity)
                    throw new Exception("Exceeds stock");

                item.Quantity += quantity;
            }
            else
            {
                _context.CartEntries.Add(new CartEntry
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(string userId, int productId, int quantity)
        {
            var cart = await GetOrCreateCart(userId);

            var item = await _context.CartEntries
                .FirstOrDefaultAsync(i =>
                    i.CartId == cart.Id &&
                    i.ProductId == productId);

            if (item == null)
                throw new Exception("Item not found");

            if (quantity <= 0)
            {
                _context.CartEntries.Remove(item);
            }
            else
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                    throw new Exception("Product not found");

                if (product.Stock < quantity)
                    throw new Exception("Not enough stock");

                item.Quantity = quantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await GetOrCreateCart(userId);

            var item = await _context.CartEntries
                .FirstOrDefaultAsync(i =>
                    i.CartId == cart.Id &&
                    i.ProductId == productId);

            if (item == null)
                return false;

            _context.CartEntries.Remove(item);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetOrCreateCart(userId);

            var items = await _context.CartEntries
                .Where(i => i.CartId == cart.Id)
                .ToListAsync();

            _context.CartEntries.RemoveRange(items);

            await _context.SaveChangesAsync();
        }


    }
}
