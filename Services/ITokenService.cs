using System.Security.Claims;

namespace NoobProject.Services {
    public interface ITokenService {
        string GenerateToken(IEnumerable<Claim> claims);
    }

}
