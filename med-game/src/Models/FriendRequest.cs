namespace med_game.src.Models
{
    public class FriendRequest
    {
        public long Id { get; set; }
        public User Author { get; set; }
        public User Subscriber { get; set; }
    }
}
