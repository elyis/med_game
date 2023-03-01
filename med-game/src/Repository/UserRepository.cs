using med_game.Models;
using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities.Request;
using med_game.src.models;
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

        public async Task<bool> ApplyForFriendship(long id, string friendEmail)
        {
            bool isSameUsers = await IsSameUsersAsync(id, friendEmail);
            if (isSameUsers == true)
                return false;

            var subscriber = await GetAsync(id);
            var author = await GetAsync(friendEmail);
            if (subscriber == null || author == null)
                return false;

            FriendRequest? friendRequest = await GetFriendRequest(id, friendEmail);
            if (friendRequest == null)
            {
                author.FriendRequestToMe.Add(new FriendRequest { Subscriber = subscriber });
                _context.SaveChanges();
            }
            return true;
        }

        public async Task<FriendRequest?> GetFriendRequest(long id, string friendEmail)
        {
            var user = await GetAsyncWithSubscriber(id);
            var friend = await GetAsyncWithSubscriber(friendEmail);

            if(user == null || friend == null) 
                return null;


            FriendRequest friendRequestToUser = new FriendRequest { Author = user, Subscriber = friend };
            FriendRequest friendRequestFromUser = new FriendRequest { Author = friend, Subscriber = user };

            FriendRequest? userInToMe = user.FriendRequestToMe.FirstOrDefault(f => f.AuthorId == user.Id && f.SubscriberId == friend.Id);
            FriendRequest? userInFromMe = user.FriendRequestFromMe.FirstOrDefault(f => f.AuthorId == friend.Id && f.SubscriberId == user.Id);

            if(userInToMe == null && userInFromMe == null)
                return null;

            if (userInFromMe == null)
                return userInToMe;
            return userInFromMe;
        }

        public async Task<User?> GetAsyncWithSubscriber(long id)
            => await _context.Users
                .Include(u => u.FriendRequestFromMe)
                .Include(u => u.FriendRequestToMe)
            .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User?> GetAsyncWithSubscriber(string email)
        => await _context.Users
                .Include(u => u.FriendRequestFromMe)
                .Include(u => u.FriendRequestToMe)
            .FirstOrDefaultAsync(u => u.Email == email);

        public async Task<bool> CancelTheFriendshipRequestAsync(long id, string friendEmail)
        {
            bool isSameUsers = await IsSameUsersAsync(id, friendEmail);
            if (isSameUsers)
                return true;

            FriendRequest? requestForFriendship = await GetFriendRequest(id, friendEmail);
            if (requestForFriendship == null)
                return false;

            var user = await GetAsyncWithSubscriber(id);
            if(user == null) 
                return false;

            bool isRemovedToMe = user.FriendRequestToMe.Remove(requestForFriendship);
            bool isRemoveFromMe = user.FriendRequestFromMe.Remove(requestForFriendship);
            if (isRemoveFromMe || isRemovedToMe)
            {
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> IsSameUsersAsync(long id, string email)
        {
            var result = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Id == id);
            return result != null ? true : false;
        }

        public Task<bool> ChangeSubscriberToFriend(long id, string subscriberEmail)
        {
            throw new NotImplementedException();
        }

        //public async Task<Friend?> GetFriendAsync(long id, string friendEmail)
        //{
        //    User? user = await _context.Users
        //        .Include(u => u.Friends)
        //        .FirstOrDefaultAsync(u => u.Id == id);

        //    User? friend = await 

        //    if (user == null) 
        //        return null;

        //    return user.Friends.FirstOrDefault(f => f.A)
        //}
    }


}
