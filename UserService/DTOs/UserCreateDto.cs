namespace UserService.DTOs
{
    // DTO клас – използва се при създаване на нов потребител през API
    public class UserCreateDto
    {
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
