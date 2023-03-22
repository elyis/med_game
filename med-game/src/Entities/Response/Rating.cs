namespace med_game.src.Entities.Response
{
    public class Rating
    {
        public Department Department { get; set; } = Department.Anatomy;
        public List<RatingInfo> listPlayers { get; set; }
    }
}
