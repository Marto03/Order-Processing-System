using UserService.Data;
using UserService.Models;

namespace UserService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync() => await Task.FromResult(_context.Users.ToList());

        public async Task<User?> GetByIdAsync(int id) => await _context.Users.FindAsync(id);

        public async Task CreateAsync(User user)
        {
            _context.Users.Add(user);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
