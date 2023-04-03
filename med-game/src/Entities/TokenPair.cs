
namespace med_game.src.Entities
{
    public class TokenPair
    {
        public TokenPair(string accessToken, string refreshToken)
        {
            access_token = "Bearer " + accessToken;
            refresh_token = refreshToken;
        }

        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }
}
