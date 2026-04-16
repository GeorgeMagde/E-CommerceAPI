namespace NoobProject.Dtos.AuthDtos {
    public class AuthResponseDto {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public string? Token { get; set; }

        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }

        public string? Role { get; set; }
    }
}
