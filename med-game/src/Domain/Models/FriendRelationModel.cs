
namespace med_game.src.Domain.Models
{
    public class FriendRelationModel
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }

        public UserModel User { get; set; }
        public UserModel Friend { get; set; }
    }
}