using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NoobProject.Dtos.AuthDtos;
using NoobProject.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NoobProject.Services {
    public class UserService : IUserService {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        public UserService(UserManager<AppUser> userManager, ITokenService tokenService) {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto model) {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return new AuthResponseDto { IsSuccess = false, Message = "User already exists!" };

            AppUser user = new() {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) {
                var firstError = result.Errors.FirstOrDefault()?.Description;
                return new AuthResponseDto { IsSuccess = false, Message = firstError ?? "User creation failed!" };
            }

            // Assign "User" role
            await _userManager.AddToRoleAsync(user, "User");

            return new AuthResponseDto { IsSuccess = true, Message = "User created successfully!" };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto model) {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid email or password." };

            if (!user.IsActive)
                return new AuthResponseDto { IsSuccess = false, Message = "Your account has been deactivated." };

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles) {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = _tokenService.GenerateToken(authClaims);

            string primaryRole = userRoles.Contains("Admin") ? "Admin" : "User";

            return new AuthResponseDto {
                IsSuccess = true,
                Token = token,
                Message = "Login successful",
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = primaryRole 
            };
        }
    }
}
