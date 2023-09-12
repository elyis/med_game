using med_game.src.Domain.Entities.Request;
using med_game.src.Domain.Entities.Response;
using med_game.src.Domain.Enums;
using med_game.src.Domain.IRepository;
using med_game.src.Domain.Models;
using med_game.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace med_game.src.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<UserModel?> AddAsync(SignUpBody body, string role)
        {
            var user = await GetAsync(body.Mail);
            if (user != null)
                return null;

            var model = new UserModel 
            { 
                Email = body.Mail, 
                Password = body.Password, 
                Nickname = body.Nickname,
                RoleName = role,
            };

            var result = await _context.Users.AddAsync(model);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<UserModel?> GetAsync(long id)
            => await _context.Users.FindAsync(id);

        public async Task<UserModel?> GetAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<UserModel?> GetByTokenAsync(string refreshTokenHash)
            => await _context.Users.FirstOrDefaultAsync(u => u.TokenHash == refreshTokenHash);


        public async Task<bool> UpdateTokenAsync(string refreshTokenHash, string email)
        {
            var user = await GetAsync(email);
            if (user == null)
                return false;

            user.TokenHash = refreshTokenHash;
            user.TokenValidBefore = DateTime.UtcNow.AddDays(15);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApplyForFriendshipAsync(long id, string subscriptionEmail)
        {
            var userSubscription = await GetSubscriptionAsync(id, subscriptionEmail);

            if (userSubscription == null)
            {
                var user = await GetAsync(id);
                var subscription = await GetSubscribersAsync(subscriptionEmail);

                subscription.Subscribers.Add(new SubscriberRelationModel { Subscriber = user });
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<SubscriberRelationModel?> GetSubscriberAsync(long id, string subEmail)
        {
            var someUser = await GetAsync(subEmail);
            if(someUser == null)
                return null;

            var user = await GetSubscribersAsync(id);
            if(user == null) 
                return null;

            var subscriber = user.Subscribers.FirstOrDefault(sub => sub.SubscriberId == someUser.Id);
            return subscriber;
        }

        public async Task<SubscriberRelationModel?> GetSubscriptionAsync(long id, string subscriptionEmail){
            var user = await GetAsync(id);
            if(user == null)
                return null;

            var subscription = await GetSubscribersAsync(subscriptionEmail);
            if(subscription == null)
                return null;

            var userSubscription = subscription.Subscribers.FirstOrDefault(e => e.SubscriberId == user.Id);
            return userSubscription;
        }

        public async Task<UserModel?> GetSubscribersAsync(long id)
            => await _context.Users
                .Include(u => u.Subscribers)
                .Include(u => u.Subscriptions)
            .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<UserModel?> GetSubscribersAsync(string email)
        => await _context.Users
                .Include(u => u.Subscribers)
                .Include(u => u.Subscriptions)
            .FirstOrDefaultAsync(u => u.Email == email);

        public async Task<SubscriberRelationModel?> RemoveSubscriptionAsync(long id, string subscriptionEmail)
        {
            var userSubscription = await GetSubscriptionAsync(id, subscriptionEmail);
            var user = await GetSubscribersAsync(subscriptionEmail);

            return 
                userSubscription != null && 
                user != null && 
                user.Subscribers.Remove(userSubscription)
                    ? (await _context.SaveChangesAsync() > 0 ? userSubscription : null)
                    : null;
        }

        public async Task<SubscriberRelationModel?> RemoveSubscriberAsync(long id, string subscriberEmail){
            var subscriber = await GetSubscriberAsync(id, subscriberEmail);
            var user = await GetSubscribersAsync(id);

            return 
                subscriber != null && 
                user != null && 
                user.Subscribers.Remove(subscriber)
                    ? (await _context.SaveChangesAsync() > 0 ? subscriber : null)
                    : null;
        }

        public async Task<bool> AddFriendAsync(long id, string subscriberEmail)
        {
            var subscriberRelation = await RemoveSubscriberAsync(id, subscriberEmail);
            if(subscriberRelation != null){
                var user = await GetFriendsAsync(id);
                var newFriend = await GetAsync(subscriberEmail);

                user.FriendsAcceptedByMe.Add(new FriendRelationModel { Friend = newFriend});
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<FriendRelationModel?> GetFriendAsync(long id, string friendEmail)
        {
            var user = await GetFriendsAsync(id);
            var friend = await GetAsync(friendEmail);

            if (user == null || friend == null)
                return null;

            var acceptedMe = user.FriendsAcceptedMe.FirstOrDefault(u => u.UserId == friend.Id && u.FriendId == user.Id);
            if(acceptedMe != null) 
                return acceptedMe;

            var acceptedByMe = user.FriendsAcceptedByMe.FirstOrDefault(u => u.UserId == user.Id && u.FriendId == friend.Id);
            if(acceptedByMe != null) 
                return acceptedByMe;
            return null;
        }

        //Удаление из друзей -> переход в подписчики
        public async Task<bool> RemoveFriendAsync(long id, string friendEmail)
        {
            var friendRelation = await GetFriendAsync(id, friendEmail);
            if (friendRelation == null)
                return false;

            var user = await GetFriendsAsync(id);
            var subscriber = await GetAsync(friendEmail);

            bool isRemovedFromMe = user.FriendsAcceptedMe.Remove(friendRelation);
            bool isRemovedFromByMe = user.FriendsAcceptedByMe.Remove(friendRelation);
            if (!isRemovedFromByMe && !isRemovedFromMe)
                return false;

            user.Subscribers.Add(new SubscriberRelationModel { Subscriber = subscriber });
            await _context.SaveChangesAsync();

            return isRemovedFromMe || isRemovedFromByMe;
        }

        public async Task<UserModel?> GetFriendsAsync(long id)
            => await _context.Users
                .Include(u => u.FriendsAcceptedByMe)
                .Include(u => u.FriendsAcceptedMe)
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<UserModel?> GetFullAsync(long id)
            => await _context.Users
                .Include(u => u.Subscriptions)
                    .ThenInclude(u => u.User)
                .Include(u => u.Subscribers)
                    .ThenInclude(u => u.Subscriber)
                .Include(u => u.FriendsAcceptedByMe)
                    .ThenInclude(u => u.Friend)
                .Include(u => u.FriendsAcceptedMe)
                    .ThenInclude(u => u.User)

                .FirstOrDefaultAsync(u => u.Id == id);

        private async Task<IEnumerable<UserRating>> GetUserRating(int count){
            return await _context.Users
                    .OrderByDescending(u => u.Rating)
                    .Select(u => u.ToUserRating())
                    .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<FriendInfo>> GetFriendsAndSubscibersInfo(long id, int count = 100)
        {
            var user = await GetFullAsync(id);
            var friendsAndSubscribers = new List<FriendInfo>();

            if (user != null)
            {
                var friendsAcceptedMe = user.FriendsAcceptedMe.Select(e => e.User.ToFriendInfo(FriendStatus.Friend));
                var friendsAcceptedByMe = user.FriendsAcceptedByMe.Select(e => e.Friend.ToFriendInfo(FriendStatus.Friend));

                var subscribers = user.Subscribers.Select(e => e.Subscriber.ToFriendInfo(FriendStatus.Subscriber));
                var subscriptions = user.Subscriptions.Select(e => e.User.ToFriendInfo(FriendStatus.ApplicationSent));

                friendsAndSubscribers = friendsAcceptedMe
                    .Concat(friendsAcceptedByMe)
                    .Concat(subscribers)
                    .Concat(subscriptions)
                    .OrderByDescending(e => e.NumberPointsInRatingDepartment)
                    .ToList();
                if(friendsAndSubscribers.Count == 0)
                    return friendsAndSubscribers;

                
                var userRating = await GetUserRating(count);
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
                return friendsAndSubscribers;
            }

            return friendsAndSubscribers;
        }

        public async Task<IEnumerable<UserInfo>> GetUsersAsync(long id, string nicknamePattern)
        {
            var users = new List<UserInfo>();
            var user = await GetFullAsync(id);

            if (user == null)
                return users;

            var playersByRating = await _context.Users
                .Where(u => EF.Functions.Like(u.Nickname.ToLower(), $"%{nicknamePattern.ToLower()}%"))
                .OrderByDescending(u => u.Rating)
                .ToListAsync();

            foreach (var player in playersByRating)
            {
                UserInfo userInfo;
                var friendAcceptedMe = user.FriendsAcceptedMe.FirstOrDefault(u => u.UserId == player.Id);
                if (friendAcceptedMe != null)
                    userInfo = friendAcceptedMe.User.ToUserInfo(UserStatus.Friend);
                else
                {
                    var friendAcceptedByMe = user.FriendsAcceptedByMe.FirstOrDefault(u => u.FriendId == player.Id);
                    if (friendAcceptedByMe != null)
                        userInfo = friendAcceptedByMe.Friend.ToUserInfo(UserStatus.Friend);
                    else
                    {
                        var subscriber = user.Subscribers.FirstOrDefault(u => u.SubscriberId == player.Id);
                        if (subscriber != null)
                            userInfo = subscriber.Subscriber.ToUserInfo(UserStatus.Subscriber);
                        else
                        {
                            var applicationSent = user.Subscriptions.FirstOrDefault(u => u.UserId == player.Id);
                            if (applicationSent != null)
                                userInfo = applicationSent.User.ToUserInfo(UserStatus.ApplicationSent);
                            else
                                userInfo = player.ToUserInfo(UserStatus.NotFriends);
                        }
                    }
                }

                users.Add(userInfo);
            }

            users = users.OrderByDescending(u => u.NumberPointsInRatingDepartment).ToList();

            var userRating = await GetUserRating(100);
            int placeInDepartment = 1;
            int currentIndexOfUserRating = 0;
            foreach(var u in userRating)
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
            return users;
        }

        public async Task<ProfileBody?> GetProfileAsync(long id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            return user?.ToProfileBody();
        }

        public async Task<bool> UpdateImageAsync(long id, string filename)
        {
            var user = await GetAsync(id);
            if(user == null) 
                return false;

            filename = filename.Replace(".jpg", ".jpeg");
            user.Image = filename;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Rating> GetRatingInfo(int count=100)
        {
            var ratingTable = await _context.Users
                .OrderByDescending(u => u.Rating)
                .Select(u => u.ToRatingInfo())
                .Take(count)
                .ToListAsync();
                
            int placeInDepartment = 1;
            foreach(var ratingInfo in ratingTable)
                ratingInfo.PlaceInRating = placeInDepartment++;
            return new Rating { ListPlayers = ratingTable };
        }

        public async Task UpdateRating(long id, int countPoints)
        {
            UserModel? user = await GetAsync(id);
            if( user == null ) 
                return;

            user.Rating += countPoints;
            if (user.Rating < 0)
                user.Rating = 0;
            await _context.SaveChangesAsync();
        }

        public async Task<FriendInfo?> GetFriendInfo(long id, long friendId)
        {
            var user = await GetAsync(id);
            var friend = await GetAsync(friendId);
            if (user == null || friend == null)
                return null;

            FriendInfo friendInfo;
            if (user.FriendsAcceptedByMe.FirstOrDefault(e => e.UserId == friend.Id) != default)
                friendInfo = friend.ToFriendInfo(FriendStatus.Friend);
            else if (user.FriendsAcceptedMe.FirstOrDefault(e => e.FriendId == friend.Id) != default)
                friendInfo = friend.ToFriendInfo(FriendStatus.Friend);
            else if (user.Subscribers.FirstOrDefault(e => e.SubscriberId == friend.Id) != default)
                friendInfo = friend.ToFriendInfo(FriendStatus.Subscriber);
            else
                friendInfo = friend.ToFriendInfo(FriendStatus.ApplicationSent);

            var ratingTable = _context.Users
                .OrderByDescending(u => u.Rating)
                .Select(u => new
            {
                u.Email,
                u.Rating
            }).ToList();

            var placeInRating = 1;
            foreach(var player in ratingTable)
            {
                if (player.Email == friendInfo.Email)
                {
                    friendInfo.PlaceInRatingDepartment = placeInRating;
                    break;
                }
                placeInRating++;
            }
            return friendInfo;
        }
    }
}