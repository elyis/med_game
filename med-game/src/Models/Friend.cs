using med_game.src.Models;

namespace med_game.src.models
{
    public class Friend
    {
        public long AuthorId { get; set; }
        public long SubscriberId { get; set; }

        public User Author { get; set; }
        public User Subscriber { get; set; }
    }
}