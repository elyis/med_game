using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Models;

namespace med_game.src.Repository
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly AppDbContext _context;

        public FriendRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<FriendRequest?> AddAsync(User author, User subscriber)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FriendRequest> GetAll(User author)
        {
            throw new NotImplementedException();
        }

        public async Task<FriendRequest?> GetAsync(long id)
            => await _context.FriendRequests
                .FindAsync(id);

        public async Task<bool> RemoveAsync(long id)
        {
            FriendRequest? friendRequest = await GetAsync(id);
            if (friendRequest == null)
                return false;

            var result = _context.FriendRequests.Remove(friendRequest);
            _context.SaveChanges();
            return result == null ? false : true;
        }

        public Task<bool> RemoveAsync(string authorEmail, string subscriberEmail)
        {
            throw new NotImplementedException();
        }
    }
}
