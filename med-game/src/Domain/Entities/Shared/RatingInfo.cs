namespace med_game.src.Domain.Entities.Shared
{
    public class RatingInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public int NumberPointsInRatingDepartment { get; set; }
        public string Icon { get; set; } = string.Empty;
        public int PlaceInRating { get; set; }
    }
}