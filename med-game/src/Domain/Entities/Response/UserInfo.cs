using med_game.src.Domain.Enums;

namespace med_game.src.Domain.Entities.Response
{
    public class UserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int NumberPointsInRatingDepartment { get; set; }
        public Department Department { get; set; }
        public int PlaceInRatingDepartment { get; set; } = 99999;
        public UserStatus Status { get; set; }
    }
}