using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoobProject.Dtos.AuthDtos;
using NoobProject.Services;

namespace NoobProject.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IUserService _userService;

        public AuthController(IUserService userService) {
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] RegisterDto model) {

            if (!ModelState.IsValid) {
                var errorMessages = ModelState.Values
                                              .SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new AuthResponseDto {
                    IsSuccess = false,
                    Errors = errorMessages
                });
            }

            var result = await _userService.RegisterAsync(model);

            if (!result.IsSuccess) {
                return BadRequest(new AuthResponseDto {
                    IsSuccess = false,
                    Errors = new List<string> { result.Message! }
                });
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model) {

            if (!ModelState.IsValid) {
                var errorMessages = ModelState.Values
                                              .SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new AuthResponseDto {
                    IsSuccess = false,
                    Errors = errorMessages
                });
            }

          
            var result = await _userService.LoginAsync(model);

            if (!result.IsSuccess) {
                return Unauthorized(new AuthResponseDto {
                    IsSuccess = false,
                    Errors = new List<string> { result.Message! }
                });
            }


            return Ok(result);
        }
    }
}