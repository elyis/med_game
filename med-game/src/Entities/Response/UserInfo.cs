using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace med_game.src.Entities.Response
{
    public class UserInfo
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int NumberPointsInRatingDepartment { get; set; }
        public Department Department { get; set; }
        public int PlaceInRatingDepartment { get; set; }
        public UserStatus Status { get; set; }
    }


    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserStatus
    {
        Friend,
        ApplicationSent,
        Subscriber,
        NotFriends,
    }
}
