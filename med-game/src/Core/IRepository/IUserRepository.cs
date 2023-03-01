using med_game.Models;
using med_game.src.Entities.Request;
using med_game.src.Models;

namespace med_game.src.Core.IRepository
{
    public interface IUserRepository : IDisposable
    {
        Task<User?> AddAsync(RegistrationBody registrationBody, string role);
        Task<User?> GetAsync(long id);
        Task<User?> GetAsync(string email);
        Task<User?> GetAsyncWithSubscriber(long id);
        Task<User?> GetAsyncWithSubscriber(string email);

        IEnumerable<User> GetAll();

        Task<bool> RemoveAsync(long id);
        Task<bool> RemoveAsync(string email);

        Task<bool> UpdateTokenAsync(string refreshTokenHash, long id);
        Task<bool> UpdateTokenAsync(string refreshToken, string email);

        Task<User?> LoginAsync(Login login);
        Task AddAchievementToEveryone(Achievement achievement);

        Task<FriendRequest?> GetFriendRequest(long id, string friendEmail);
        Task<bool> ApplyForFriendship(long id, string friendEmail);
        Task<bool> CancelTheFriendshipRequestAsync(long id, string friendEmail);
        Task<bool> IsSameUsersAsync(long id, string email);

        Task<bool> ChangeSubscriberToFriend(long id, string subscriberEmail);
        //Task<Friend?> GetFriendAsync(long id, string friendEmail);
    }
}
