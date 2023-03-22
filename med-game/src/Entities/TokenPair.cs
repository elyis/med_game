
namespace med_game.src.Entities
{
    public class TokenPair
    {
        public TokenPair(string accessToken, string refreshToken)
        {
            Access_token = "Bearer " + accessToken;
            Refresh_token = refreshToken;
        }

        public string Access_token { get; set; }
        public string Refresh_token { get; set; }
    }
}
