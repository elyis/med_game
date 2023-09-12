using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Enums;

namespace med_game.src.Domain.Entities.Response
{
    public class Rating
    {
        public Department Department { get; set; } = Department.Anatomy;
        public List<RatingInfo> ListPlayers { get; set; } = new();
    }
}