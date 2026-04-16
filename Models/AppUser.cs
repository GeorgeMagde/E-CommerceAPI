using Microsoft.AspNetCore.Identity;

namespace NoobProject.Models {
    public class AppUser:IdentityUser {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
