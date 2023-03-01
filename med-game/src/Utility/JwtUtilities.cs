using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace med_game.src.Utility
{
    public class JwtUtilities
    {
        public IEnumerable<Claim> GetClaimsFromJwt(string token)
        {
            string tokenWithoutSchema = token.Replace("Bearer ", string.Empty);
            return new JwtSecurityTokenHandler()
                    .ReadJwtToken(tokenWithoutSchema)
                    .Claims;
        }
        

        public string? GetClaimUserId(string token) 
            =>
            GetClaimsFromJwt(token)
                .FirstOrDefault(claim => claim.Type == "UserId")
                ?.Value;
       
    }
}
