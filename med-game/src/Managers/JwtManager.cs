using med_game.src.Core.IManager;
using med_game.src.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace med_game.src.Managers
{
    public class JwtManager : IJwtManager
    {
        private readonly string key;
        private readonly SigningCredentials _signingCredentials;
        private readonly HMACSHA512 _hmac512;

        public JwtManager()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
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
                    expires: DateTime.UtcNow.AddHours(2),
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

