namespace med_game.src.Domain.Models
{
    public class SubscriberRelationModel
    {
        public long UserId { get; set; }
        public UserModel User { get; set; }
        
        public long SubscriberId { get; set; }
        public UserModel Subscriber { get; set; }
    }
}