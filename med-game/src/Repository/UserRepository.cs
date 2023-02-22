using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities.Request;
using med_game.src.Models;
using Microsoft.EntityFrameworkCore;

namespace med_game.src.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public Task<User?> AddAsync(RegistrationBody registrationBody)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
            => _context.Users;

        public async Task<User?> GetAsync(long id)
            => await _context.Users.FindAsync(id);

        public async Task<User?> GetAsync(string email)
            => await _context.Users.FindAsync(email);

        public async Task<bool> RemoveAsync(long id)
        {
            User? user = await GetAsync(id);
            if(user == null)
                return false;

            var result = _context.Users.Remove(user);
            return result == null ? false : true;
        }

        public async Task<bool> RemoveAsync(string email)
        {
            User? user = await GetAsync(email);
            if (user == null)
                return false;

            var result = _context.Users.Remove(user);
            return result == null ? false : true;
        }

        public async Task<bool> UpdateToken(string refreshTokenHash, long id)
        {
            User? user = await GetAsync(id);
            if(user == null)
                return false;

            user.TokenHash = refreshTokenHash;
            user.TokenValidBefore= DateTime.UtcNow.AddDays(15);

            _context.SaveChanges();
        }

        public Task<bool> UpdateToken(string refreshToken, string email)
        {
            throw new NotImplementedException();
        }
    }
}
