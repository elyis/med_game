using System.Text.Json.Serialization;

namespace med_game.src.Entities.Response
{
    public class FriendInfo
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string? Icon { get; set; }
        public int NumberPointsInRatingDepartment { get; set; }
        public Department Department { get; set; } = Department.Anatomy;
        public int PlaceInRatingDepartment { get; set; }
        public FriendStatus Status { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Department
    {
        Anatomy,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FriendStatus
    {
        Friend,
        ApplicationSent,
        Subscriber
    }

}
