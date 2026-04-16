using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoobProject.Dtos.AdminDtos;
using NoobProject.Dtos.AuthDtos;
using NoobProject.Dtos.ProductDtos;
using NoobProject.Models;
using NoobProject.Services;
using System.Security.Claims;

namespace NoobProject.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="Admin")]

    public class AdminController : ControllerBase {
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;

        public AdminController(UserManager<AppUser> userManager, IProductService productService) {
            _userManager = userManager;
            _productService = productService;
        }

        #region User Management
       
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers() {
            var users = await _userManager.Users
                .Select(u => new {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.IsActive
                })
                .ToListAsync();
            return Ok(users);
        }

        [HttpPost("users")]
        public async Task<IActionResult> AddUser([FromBody] RegisterDto model) {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return BadRequest("User already exists!");

            AppUser user = new() {
                Email = model.Email,
                UserName = model.Email,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded) {
                return Ok(new { Message = "User created successfully." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPut("users/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto model) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            if (!string.IsNullOrWhiteSpace(model.Name))
                user.Name = model.Name;

            if (!string.IsNullOrWhiteSpace(model.Email)) {
                user.Email = model.Email;
                user.UserName = model.Email;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok(new { Message = "User updated successfully." });

            return BadRequest(result.Errors);
        }

        [HttpPut("toggle-status/{userId}")]
        public async Task<IActionResult> ToggleUserStatus(string userId) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user.Id == currentUserId) return BadRequest("You cannot deactivate your own account.");

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok(new { Message = $"User {(user.IsActive ? "activated" : "deactivated")} successfully." });

            return BadRequest(result.Errors);
        }

        #endregion

        #region Product Management

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters) {
            var products = await _productService.GetProductsAsync(queryParameters, Request.Scheme, Request.Host.Value);
            return Ok(products);
        }

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateUpdateProductDto dto) {
            var product = await _productService.CreateProductAsync(dto, Request.Scheme, Request.Host.Value);
            return Ok(product);
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDto dto) {
            var product = await _productService.UpdateProductAsync(id, dto, Request.Scheme, Request.Host.Value);
            if (product == null) return NotFound("Product not found");
            return Ok(product);
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id) {
            var success = await _productService.DeleteProductAsync(id);
            if (!success) return NotFound("Product not found");
            return Ok(new { Message = "Product deleted successfully." });
        }

        #endregion
    }
}
