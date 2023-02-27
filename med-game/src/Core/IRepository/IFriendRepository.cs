using med_game.src.Models;

namespace med_game.src.Core.IRepository
{
    public interface IFriendRepository
    {
        Task<Friend?> AddAsync(User author, User subscriber);
        Task<FriendRequest?> GetAsync(long id);
        Task<bool> RemoveAsync(long id);
        Task<bool> RemoveAsync(string authorEmail, string subscriberEmail);
        IEnumerable<Friend> GetAll(User author);
    }
}
