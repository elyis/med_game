using System.Security.Claims;
using med_game.src.Domain.Entities.Shared;

namespace med_game.src.Application.IManager
{
    public interface IJwtManager
    {
        TokenPair GenerateTokenPair(List<Claim> claims);
        string GenerateAccessToken(List<Claim> claims);
        string GenerateRefreshToken();
        string ComputeRefreshHashToken(string guidToken);
    }
}