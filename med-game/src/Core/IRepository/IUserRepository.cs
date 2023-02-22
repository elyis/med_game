using med_game.src.Entities.Request;
using med_game.src.Models;

namespace med_game.src.Core.IRepository
{
    public interface IUserRepository : IDisposable
    {
        Task<User?> AddAsync(RegistrationBody registrationBody);
        Task<User?> GetAsync(long id);
        Task<User?> GetAsync(string email);
        IEnumerable<User> GetAll();

        Task<bool> RemoveAsync(long id);
        Task<bool> RemoveAsync(string email);

        Task<bool> UpdateToken(string refreshTokenHash, long id);
        Task<bool> UpdateToken(string refreshToken, string email);
    }
}
