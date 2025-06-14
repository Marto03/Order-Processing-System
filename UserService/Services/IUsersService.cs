using Shared.DTOs;
using UserService.Models;

namespace UserService.Services
{
    public interface IUsersService
    {
        Task<User?> CreateUserAsync(UserCreateDto dto);
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
