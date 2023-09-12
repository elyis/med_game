

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using med_game.src.Application.IManager;
using med_game.src.Domain.Entities.Shared;
using Microsoft.IdentityModel.Tokens;

namespace med_game.src.Application.Managers
{
    public class JwtManager : IJwtManager
    {
        private readonly string key;
        private readonly SigningCredentials _signingCredentials;
        private readonly HMACSHA512 _hmac512;

        public JwtManager(IConfiguration config)
        {
            var jsonSettings = config.GetSection("JwtSettings");
            key = jsonSettings.GetValue<string>("Key")!;

            _hmac512 = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            _signingCredentials = new SigningCredentials
                (
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha512
                );
        }
        public TokenPair GenerateTokenPair(List<Claim> claims)
        {
            return new TokenPair(
                accessToken: GenerateAccessToken(claims),
                refreshToken: GenerateRefreshToken()
                );
        }

        public string GenerateAccessToken(List<Claim> claims)
        {
            var accessToken = new JwtSecurityToken
                (
                    claims: claims,
                    expires: DateTime.UtcNow.AddYears(1),
                    signingCredentials: _signingCredentials
                );
            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }

        public string GenerateRefreshToken() => Guid.NewGuid().ToString();
        public string ComputeRefreshHashToken(string guidToken)
        {
            byte[] bytes = _hmac512.ComputeHash(Encoding.UTF8.GetBytes(guidToken));
            return Convert.ToBase64String(bytes);
        }
    }
}