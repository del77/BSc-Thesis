using System.Linq;
using System.Threading.Tasks;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public interface IUsersRepository
    {
        Task CreateUserAsync(User user);
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> ExistsUserByUsername(string username);
    }
    public class UsersesRepository : IUsersRepository
    {
        private readonly Context _context;

        public UsersesRepository(Context context)
        {
            _context = context;
        }

        public async Task CreateUserAsync(User user)
        { 
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsUserByUsername(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}