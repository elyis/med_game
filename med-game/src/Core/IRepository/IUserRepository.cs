using med_game.Models;
using med_game.src.Entities.Request;
using med_game.src.Entities.Response;
using med_game.src.models;
using med_game.src.Models;

namespace med_game.src.Core.IRepository
{
    public interface IUserRepository : IDisposable
    {
        Task<User?> AddAsync(RegistrationBody registrationBody, string role);
        Task<User?> GetAsync(long id);
        Task<User?> GetAsync(string email);
        Task<User?> GetSubscribersAsync(long id);
        Task<User?> GetSubscribersAsync(string email);

        IEnumerable<User> GetAll();

        Task<bool> RemoveAsync(long id);
        Task<bool> RemoveAsync(string email);

        Task<bool> UpdateTokenAsync(string refreshTokenHash, long id);
        Task<bool> UpdateTokenAsync(string refreshToken, string email);
        Task<bool> UpdateImageAsync(long id, string filename);
        Task<bool> UpdateImageAsync(string email, string filename);

        Task<User?> LoginAsync(Login login);
        Task AddAchievementToEveryone(Achievement achievement);

        Task<FriendRequest?> GetFriendRequest(long id, string friendEmail);
        Task<bool> ApplyForFriendship(long id, string friendEmail);
        Task<FriendRequest?> RemoveFriendRequestAsync(long id, string friendEmail);
        Task<bool> IsSameUsersAsync(long id, string email);

        Task<bool> AddFriend(long id, string subscriberEmail);
        Task<Friends?> GetFriendAsync(long id, string friendEmail);
        Task<bool> RemoveFriend(long id, string friendEmail);

        Task<User?> GetFriendsAsync(long id);
        Task<User?> GetFriendsAsync(string email);

        Task<FriendRequest?> GetFriendRequestFrom(long id, string friendEmail);
        Task<FriendRequest?> GetFriendRequestTo(long id, string friendEmail);
        Task<User?> GetFullAsync(long id);
        Task<User?> GetFullAsync(string email);

        Task<IEnumerable<FriendInfo>> GetFriendsAndSubscibersInfo(long id);
        Task<IEnumerable<UserInfo>> GetUsers(long id, string nicknamePattern);

        Task<ProfileBody?> GetProfileAsync(long id);
        Task<ProfileBody?> GetProfileAsync(string email);
        IEnumerable<RatingInfo> GetRatingInfo();
        Task UpdateRating(long id, int countPoints);
    }
}
