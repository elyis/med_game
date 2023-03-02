namespace med_game.src.Models
{
    public class FriendRequest
    {
        public long SubscriberId { get; set; }
        public long UserId { get; set; }

        public User Subscriber { get; set; }
        public User User { get; set; }
    }
}
