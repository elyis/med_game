using med_game.src.Domain.Entities.Response;

namespace med_game.src.Domain.Entities.Game
{
    public class RequestForFight
    {
        public string Module { get; set; }
        public FriendInfo FriendInfo { get; set; }
        public string TokenRoom { get; set; }
    }
}