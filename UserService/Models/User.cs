namespace UserService.Models
{
    public class User
    {
        public int Id { get; set; } // Уникален идентификатор

        public string Username { get; set; } = string.Empty; // Име на потребителя

        public string Email { get; set; } = string.Empty; // Имейл адрес

        public string Role { get; set; } = "Customer"; // Тип на потребителя (Customer, Admin и др.)
    }
}
