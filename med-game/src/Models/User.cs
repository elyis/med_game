using med_game.Models;
using med_game.src.Entities;
using med_game.src.Entities.Game;
using med_game.src.Entities.Response;
using med_game.src.models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace med_game.src.Models
{
    [Index("Email", IsUnique = true)]
    [Index("Rating", "Nickname")]
    public class User
    {
        public long Id { get; private set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public int Rating { get; set; } = 0;

        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string RoleName { get; set; }
        public string? Image { get; set; }
        public string? TokenHash { get; set; }
        public DateTime? TokenValidBefore { get; set; }

        public List<Achievement> Achievements { get; set; } = new();


        public List<Friends> FriendsAcceptedMe { get; set; } = new();
        public List<Friends> FriendsAcceptedByMe { get; set; } = new();
        public List<FriendRequest> FriendRequestToMe { get; set; } = new();
        public List<FriendRequest> FriendRequestFromMe { get; set; } = new();

        public RatingInfo ToRatingInfo()
        {
            return new RatingInfo
            {
                email = Email,
                NumberPointsInRatingDepartment = Rating,
                icon = Image == null ? "" : @$"{Constants.webPathToProfileIcons}{Image}",
                Nickname = Nickname,
                PlaceInRating = 0
            };
        }

        public ProfileBody ToProfileBody()
        {
            return new ProfileBody
            {
                Nickname = Nickname,
                Email = Email,
                UrlIcon = Image == null ? "" : @$"{Constants.webPathToProfileIcons}{Image}",
                //Achievements = Achievements.Select(a => a.ToAchievementBody()).ToList()
            };
        }

        public UserInfo ToUserInfo(UserStatus status)
        {
            return new UserInfo
            {
                Email = Email,
                Icon = Image == null ? "" : @$"{Constants.webPathToProfileIcons}{Image}",
                Name = Nickname,
                Status = status,
                NumberPointsInRatingDepartment = Rating,
                PlaceInRatingDepartment = 0
            };
        }

        public FriendInfo ToFriendInfo(FriendStatus status)
        {
            return new FriendInfo
            {
                Email = Email,
                Icon = Image == null ? "" : @$"{Constants.webPathToProfileIcons}{Image}",
                Name = Nickname,
                Status = status,
                NumberPointsInRatingDepartment = Rating,
                PlaceInRatingDepartment = 0
            };
        }

        public GameStatisticInfo ToGameStatisticInfo()
        {
            return new GameStatisticInfo
            {
                Nickname = Nickname,
                Image = Image == null ? "" : @$"{Constants.webPathToProfileIcons}{Image}"
            };
        }

    }


    public enum Roles
    {
        User,
        Admin,
    }
}
