using med_game.src.Entities.Response;

namespace med_game.src.Entities.Game
{
    public class RequestForFight
    {
        public string Module { get; set; }
        public FriendInfo FriendInfo { get; set; }
        public string TokenRoom { get; set; }
    }
}
