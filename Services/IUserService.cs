using NoobProject.Dtos.AuthDtos;

namespace NoobProject.Services {
    public interface IUserService {
        Task<AuthResponseDto> RegisterAsync(RegisterDto model);
        Task<AuthResponseDto> LoginAsync(LoginDto model);
    }
}
