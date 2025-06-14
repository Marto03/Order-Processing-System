namespace Shared.DTOs
{
    // DTO клас – използва се при създаване на нов потребител през API
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; } = string.Empty;
        // По избор: имейл, роля и др.
        public string? Email { get; set; }
    }
}
