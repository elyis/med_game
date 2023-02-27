using med_game.Models;
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

        public async Task<User?> AddAsync(RegistrationBody registrationBody, string role)
        {
            User? user = await GetAsync(registrationBody.Email);
            if (user != null)
                return null;

            var model = new User 
            { 
                Email = registrationBody.Email, 
                Password = registrationBody.Password, 
                Nickname = registrationBody.Nickname,
                RoleName = role,
            };

            var result = await _context.Users.AddAsync(model);
            await _context.SaveChangesAsync();
            return result.Entity;

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
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

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

        public async Task<bool> UpdateTokenAsync(string refreshTokenHash, long id)
        {
            User? user = await GetAsync(id);
            if(user == null)
                return false;

            user.TokenHash = refreshTokenHash;
            user.TokenValidBefore= DateTime.UtcNow.AddDays(15);

            _context.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateTokenAsync(string refreshTokenHash, string email)
        {
            User? user = await GetAsync(email);
            if (user == null)
                return false;

            user.TokenHash = refreshTokenHash;
            user.TokenValidBefore = DateTime.UtcNow.AddDays(15);

            _context.SaveChanges();
            return true;
        }

        public async Task<User?> LoginAsync(Login login)
            => await _context.Users
            .FirstOrDefaultAsync
            (
                u => u.Email == login.Email
                &&
                u.Password == login.PasswordHash
            );

        public Task AddAchievementToEveryone(Achievement achievement)
        {
            var users = _context.Users;
            foreach ( var user in users )
                user.Achievements.Add(achievement);

            _context.SaveChanges();
            return Task.CompletedTask;
        }
    }
}
