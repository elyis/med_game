using med_game.src.Domain.Entities.Request;
using med_game.src.Domain.Entities.Response;
using med_game.src.Domain.Models;

namespace med_game.src.Domain.IRepository
{
    public interface IUserRepository
    {
        Task<UserModel?> AddAsync(SignUpBody body, string role);
        Task<UserModel?> GetAsync(long id);
        Task<UserModel?> GetAsync(string email);
        Task<UserModel?> GetSubscribersAsync(long id);
        Task<UserModel?> GetSubscribersAsync(string email);

        Task<bool> UpdateTokenAsync(string refreshToken, string email);
        Task<bool> UpdateImageAsync(long id, string filename);

        Task<SubscriberRelationModel?> GetSubscriberAsync(long id, string subEmail);
        Task<SubscriberRelationModel?> GetSubscriptionAsync(long id, string subscriptionEmail);
        Task<bool> ApplyForFriendshipAsync(long id, string friendEmail);
        Task<SubscriberRelationModel?> RemoveSubscriptionAsync(long id, string subscriptionEmail);
        Task<SubscriberRelationModel?> RemoveSubscriberAsync(long id, string subscriberEmail);
        Task<bool> AddFriendAsync(long id, string subscriberEmail);
        Task<FriendRelationModel?> GetFriendAsync(long id, string friendEmail);
        Task<bool> RemoveFriendAsync(long id, string friendEmail);

        Task<UserModel?> GetFriendsAsync(long id);

        Task<UserModel?> GetFullAsync(long id);
        Task<IEnumerable<FriendInfo>> GetFriendsAndSubscibersInfo(long id, int count = 100);
        Task<IEnumerable<UserInfo>> GetUsersAsync(long id, string nicknamePattern);

        Task<ProfileBody?> GetProfileAsync(long id);
        Task<UserModel?> GetByTokenAsync(string refreshTokenHash);
        Task<Rating> GetRatingInfo(int count = 100);
        Task UpdateRating(long id, int countPoints);
        Task<FriendInfo?> GetFriendInfo(long id, long friendId);
    }
}
