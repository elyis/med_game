using med_game.src.Entities;
using System.Security.Claims;

namespace med_game.src.Core.IManager
{
    public interface IJwtManager
    {
        TokenPair GenerateTokenPair(List<Claim> claims);
        string GenerateAccessToken(List<Claim> claims);
        string GenerateRefreshToken();
        string ComputeRefreshHashToken(string guidToken);
    }
}
