using med_game.Models;
using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities.Request;
using med_game.src.Entities.Response;
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
                u.Password == login.Password
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
            var user = await GetSubscribersAsync(id);
            var friend = await GetAsync(friendEmail);

            if(user == null || friend == null) 
                return null;

            FriendRequest? userInToMe = user.FriendRequestToMe.FirstOrDefault(f => f.UserId == user.Id && f.SubscriberId == friend.Id);
            FriendRequest? userInFromMe = user.FriendRequestFromMe.FirstOrDefault(f => f.UserId == friend.Id && f.SubscriberId == user.Id);

            if(userInToMe == null && userInFromMe == null)
                return null;

            if (userInFromMe == null)
                return userInToMe;
            return userInFromMe;
        }

        public async Task<FriendRequest?> GetFriendRequestFrom(long id, string friendEmail)
        {
            var user = await GetSubscribersAsync(id);
            var friend = await GetAsync(friendEmail);

            if (user == null || friend == null)
                return null;

            FriendRequest? userInFromMe = user.FriendRequestFromMe.FirstOrDefault(f => f.UserId == friend.Id && f.SubscriberId == user.Id);
            return userInFromMe;

        }

        public async Task<FriendRequest?> GetFriendRequestTo(long id, string friendEmail)
        {
            var user = await GetSubscribersAsync(id);
            var friend = await GetAsync(friendEmail);

            if (user == null || friend == null)
                return null;

            FriendRequest? userInToMe = user.FriendRequestToMe.FirstOrDefault(f => f.UserId == user.Id && f.SubscriberId == friend.Id);
            return userInToMe;
        }


        public async Task<User?> GetSubscribersAsync(long id)
            => await _context.Users
                .Include(u => u.FriendRequestFromMe)
                .Include(u => u.FriendRequestToMe)
            .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User?> GetSubscribersAsync(string email)
        => await _context.Users
                .Include(u => u.FriendRequestFromMe)
                .Include(u => u.FriendRequestToMe)
            .FirstOrDefaultAsync(u => u.Email == email);

        public async Task<FriendRequest?> RemoveFriendRequestAsync(long id, string friendEmail)
        {
            bool isSameUsers = await IsSameUsersAsync(id, friendEmail);
            if (isSameUsers)
                return null;

            FriendRequest? friendRequest = await GetFriendRequest(id, friendEmail);
            if (friendRequest == null)
                return null;

            var user = await GetSubscribersAsync(id);
            if(user == null) 
                return null;

            bool isRemovedToMe = user.FriendRequestToMe.Remove(friendRequest);
            bool isRemoveFromMe = user.FriendRequestFromMe.Remove(friendRequest);
            if (isRemoveFromMe || isRemovedToMe)
            {
                _context.SaveChanges();
                return friendRequest;
            }
            return null;
        }

        public async Task<bool> IsSameUsersAsync(long id, string email)
        {
            var result = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Id == id);
            return result != null ? true : false;
        }

        public async Task<bool> AddFriend(long id, string subscriberEmail)
        {
            if(await IsSameUsersAsync(id, subscriberEmail)) 
                return true;

            Friends? friendRelation = await GetFriendAsync(id, subscriberEmail);
            if (friendRelation != null)
                return true;


            User? user = await GetFriendsAsync(id);
            User? friend = await GetAsync(subscriberEmail);
            if (user == null || friend == null)
                return false;


            if(await GetFriendRequestTo(id, subscriberEmail) != null)
            {
                FriendRequest? friendRequest = await RemoveFriendRequestAsync(id, subscriberEmail);
                if (friendRequest == null)
                    return false;

                user.FriendsAcceptedByMe.Add(new Friends { Friend = friend });
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public async Task<Friends?> GetFriendAsync(long id, string friendEmail)
        {
            User? user = await GetFriendsAsync(id);
            User? friend = await GetAsync(friendEmail);
            if (user == null || friend == null)
                return null;

            Friends? acceptedMe = user.FriendsAcceptedMe.FirstOrDefault(u => u.UserId == friend.Id && u.FriendId == user.Id);
            if(acceptedMe != null) 
                return acceptedMe;

            Friends? acceptedByMe = user.FriendsAcceptedByMe.FirstOrDefault(u => u.UserId == user.Id && u.FriendId == friend.Id);
            if(acceptedByMe != null) 
                return acceptedByMe;
            return null;
        }

        //Удаление из друзей -> переход в подписчики
        public async Task<bool> RemoveFriend(long id, string friendEmail)
        {
            Friends? friendRelation = await GetFriendAsync(id, friendEmail);
            if (friendRelation == null)
                return false;

            User? user = await GetFriendsAsync(id);
            User? subscriber = await GetAsync(friendEmail);
            if (user == null || subscriber == null)
                return false;

            bool isRemovedFromMe = user.FriendsAcceptedMe.Remove(friendRelation);
            bool isRemovedFromByMe = user.FriendsAcceptedByMe.Remove(friendRelation);
            if (!isRemovedFromByMe && !isRemovedFromMe)
                return false;

            user.FriendRequestToMe.Add(new FriendRequest { Subscriber = subscriber });
            _context.SaveChanges();

            return isRemovedFromMe == false ? isRemovedFromByMe : isRemovedFromMe;

        }

        public async Task<User?> GetFriendsAsync(long id)
            => await _context.Users
                .Include(u => u.FriendsAcceptedByMe)
                .Include(u => u.FriendsAcceptedMe)
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User?> GetFriendsAsync(string email)
            => await _context.Users
                .Include(u => u.FriendsAcceptedByMe)
                .Include(u => u.FriendsAcceptedByMe)
                .FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetFullAsync(long id)
            => await _context.Users
                .Include(u => u.FriendRequestFromMe)
                    .ThenInclude(u => u.User)
                .Include(u => u.FriendRequestToMe)
                    .ThenInclude(u => u.Subscriber)
                .Include(u => u.FriendsAcceptedByMe)
                    .ThenInclude(u => u.Friend)
                .Include(u => u.FriendsAcceptedMe)
                    .ThenInclude(u => u.User)

                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User?> GetFullAsync(string email)
        => await _context.Users
                .Include(u => u.FriendRequestFromMe)
                    .ThenInclude(u => u.User)
                .Include(u => u.FriendRequestToMe)
                    .ThenInclude(u => u.Subscriber)
                .Include(u => u.FriendsAcceptedByMe)
                    .ThenInclude(u => u.Friend)
                .Include(u => u.FriendsAcceptedMe)
                    .ThenInclude(u => u.User)

                .FirstOrDefaultAsync(u => u.Email == email);

        //Прости меня за такой поиск друзей и подписчиков
        public async Task<IEnumerable<FriendInfo>> GetFriendsAndSubscibersInfo(long id)
        {
            List<FriendInfo> friendsAndSubscribers = new List<FriendInfo>();

            User? user = await GetFullAsync(id);
            

            if (user != null)
            {
                if(
                    user.FriendsAcceptedMe.Count == 0 && 
                    user.FriendsAcceptedByMe.Count == 0 && 
                    user.FriendRequestFromMe.Count == 0 && 
                    user.FriendRequestToMe.Count == 0
                  )
                {
                    return friendsAndSubscribers;
                }

                foreach(var friend in user.FriendsAcceptedMe)
                {
                    FriendInfo friendInfo = friend.User.ToFriendInfo(FriendStatus.Friend);
                    friendsAndSubscribers.Add(friendInfo);
                }

                foreach (var friend in user.FriendsAcceptedByMe)
                {
                    FriendInfo friendInfo = friend.Friend.ToFriendInfo(FriendStatus.Friend);
                    friendsAndSubscribers.Add(friendInfo);
                }

                foreach (var subscriber in user.FriendRequestToMe)
                {
                    FriendInfo friendInfo = subscriber.Subscriber.ToFriendInfo(FriendStatus.Subscriber);
                    friendsAndSubscribers.Add(friendInfo);
                }

                foreach (var author in user.FriendRequestFromMe)
                {
                    FriendInfo friendInfo = author.User.ToFriendInfo(FriendStatus.ApplicationSent);
                    friendsAndSubscribers.Add(friendInfo);
                }

                var userRating = _context.Users
                    .OrderByDescending(u => u.Rating)
                    .Select(u => new
                {
                    u.Email,
                    u.Rating,
                }).ToList();

                friendsAndSubscribers = friendsAndSubscribers.OrderByDescending(f => f.NumberPointsInRatingDepartment).ToList();

                int placeInDepartment = 1;
                int currentIndexOfUserRating = 0;
                foreach(var u in userRating)
                {
                    if(u.Email == friendsAndSubscribers[currentIndexOfUserRating].Email)
                    {
                        friendsAndSubscribers[currentIndexOfUserRating].PlaceInRatingDepartment = placeInDepartment;
                        currentIndexOfUserRating++;

                        if (currentIndexOfUserRating == friendsAndSubscribers.Count)
                            return friendsAndSubscribers;
                    }

                    placeInDepartment++;
                }

            }

            return friendsAndSubscribers;
        }

        /*
         * Будущий я, прости за эту хню,
         * Не учитывает, что является частью рейтинга => проблемы со статусом
         */
        public async Task<IEnumerable<UserInfo>> GetUsers(long id, string nicknamePattern)
        {
            List<UserInfo> users = new List<UserInfo>();
            User? user = await GetFullAsync(id);


            if (user != null)
            {
                var playersByRating = _context.Users.
                        Where(u => EF.Functions.Like(u.Nickname, $"%{nicknamePattern}%"))
                        .OrderByDescending(u => u.Rating);

                if(playersByRating.Count() > 0)
                {
                    foreach(var player in playersByRating)
                    {
                        Friends? friendAcceptedMe = user.FriendsAcceptedMe.FirstOrDefault(u => u.UserId == player.Id);
                        if(friendAcceptedMe != null)
                        {
                            UserInfo userInfo = friendAcceptedMe.User.ToUserInfo(UserStatus.Friend);
                            users.Add(userInfo);
                            continue;
                        }


                        Friends? friendAcceptedByMe = user.FriendsAcceptedByMe.FirstOrDefault(u => u.FriendId == player.Id);
                        if (friendAcceptedByMe != null)
                        {
                            UserInfo userInfo = friendAcceptedByMe.Friend.ToUserInfo(UserStatus.Friend);
                            users.Add(userInfo);
                            continue;
                        }

                        FriendRequest? subscriber = user.FriendRequestToMe.FirstOrDefault(u => u.SubscriberId == player.Id);
                        if (subscriber != null)
                        {
                            UserInfo userInfo = subscriber.Subscriber.ToUserInfo(UserStatus.Subscriber);
                            users.Add(userInfo);
                            continue;
                        }

                        FriendRequest? applicationSent = user.FriendRequestFromMe.FirstOrDefault(u => u.UserId == player.Id);
                        if (applicationSent != null)
                        {
                            UserInfo userInfo = applicationSent.User.ToUserInfo(UserStatus.ApplicationSent);
                            users.Add(userInfo);
                            continue;
                        }
                        UserInfo user_info = player.ToUserInfo(UserStatus.NotFriends);
                        users.Add(user_info);
                    }

                    var userRating = _context.Users.OrderByDescending(e => e.Rating).Select(u => new
                    {
                        u.Email,
                        u.Rating
                    });
                    users = users.OrderByDescending(u => u.NumberPointsInRatingDepartment).ToList();


                    int placeInDepartment = 1;
                    int currentIndexOfUserRating = 0;
                    foreach (var u in userRating)
                    {
                        if(u.Email == users[currentIndexOfUserRating].Email)
                        {
                            users[currentIndexOfUserRating].PlaceInRatingDepartment = placeInDepartment;
                            currentIndexOfUserRating++;

                            if (currentIndexOfUserRating == users.Count)
                                return users;
                        }

                        placeInDepartment++;
                    }
                }
            }

            return users;
        }

        public async Task<ProfileBody?> GetProfileAsync(long id)
        {
            User? user = await _context.Users
                .Include(u => u.Achievements)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user == null ? null : user.ToProfileBody();
        }

        public async Task<ProfileBody?> GetProfileAsync(string email)
        {
            User? user = await _context.Users
                .Include(u => u.Achievements)
                .FirstOrDefaultAsync(u => u.Email == email);

            return user == null ? null : user.ToProfileBody();
        }

        public async Task<bool> UpdateImageAsync(long id, string filename)
        {
            User? user = await GetAsync(id);
            if(user == null) 
                return false;

            filename = filename.Replace(".jpg", ".jpeg");
            user.Image = filename;
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateImageAsync(string email, string filename)
        {
            User? user = await GetAsync(email);
            if(user == null) 
                return false;

            filename = filename.Replace(".jpg", ".jpeg");
            user.Image = filename;
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<RatingInfo> GetRatingInfo()
        {
            var ratingTable = _context.Users.OrderByDescending(u => u.Rating).Select(u => u.ToRatingInfo());
            return ratingTable;
        }
    }


}
