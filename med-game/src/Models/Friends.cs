using med_game.src.Models;

namespace med_game.src.models
{
    public class Friends
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }

        public User User { get; set; }
        public User Friend { get; set; }
    }
}