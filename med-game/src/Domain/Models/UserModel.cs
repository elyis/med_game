using System.ComponentModel.DataAnnotations;
using med_game.src.Domain.Entities.Game;
using med_game.src.Domain.Entities.Response;
using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace med_game.src.Domain.Models
{
    [Index("Email", IsUnique = true)]
    [Index("Rating", "Nickname")]
    public class UserModel
    {
        public long Id { get; private set; }
        public string Email { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;

        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string RoleName { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? TokenHash { get; set; }
        public DateTime? TokenValidBefore { get; set; }


        public List<FriendRelationModel> FriendsAcceptedMe { get; set; } = new();
        public List<FriendRelationModel> FriendsAcceptedByMe { get; set; } = new();
        public List<SubscriberRelationModel> Subscribers { get; set; } = new();
        public List<SubscriberRelationModel> Subscriptions { get; set; } = new();

        public RatingInfo ToRatingInfo()
        {
            return new RatingInfo
            {
                Email = Email,
                NumberPointsInRatingDepartment = Rating,
                Icon = Image == null ? "" : @$"{Constants.webPathToProfileIcons}{Image}",
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
                PlaceInRatingDepartment = 999999
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

        public UserRating ToUserRating(){
            return new UserRating{
                Email = Email,
                Rating = Rating,
            };
        }

    }

    public class UserRating{
        public string Email { get; set; }
        public int Rating { get; set; }
    }
}