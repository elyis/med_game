namespace med_game.src.Models
{
    public class FriendRequest
    {
        public long SubscriberId { get; set; }
        public long AuthorId { get; set; }

        public User Subscriber { get; set; }
        public User Author { get; set; }
    }
}
