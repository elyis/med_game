using med_game.src.Domain.Enums;

namespace med_game.src.Domain.Entities.Response
{
    public class FriendInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int NumberPointsInRatingDepartment { get; set; }
        public Department Department { get; set; } = Department.Anatomy;
        public int PlaceInRatingDepartment { get; set; }
        public FriendStatus Status { get; set; }

        public bool Equals(FriendInfo other)
        {
            if(Email == other.Email && Name == other.Name && Icon == other.Icon)
                return true;
            return false;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as FriendInfo);
        }
    }
}