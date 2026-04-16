using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoobProject.Models;
using System.Security.Claims;

namespace NoobProject.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager) {
            _userManager = userManager;
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
    }
}
