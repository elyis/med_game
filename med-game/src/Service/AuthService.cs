using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Entities.Request;
using med_game.src.Entities;
using med_game.src.Managers;
using System.Security.Claims;
using med_game.src.Models;

namespace med_game.src.Service
{
    public class AuthService : IAuthService
    {
        private readonly JwtManager _jwtManager;
        private readonly IUserRepository _userRepository;

        public AuthService(JwtManager jwtManager, IUserRepository userRepository)
        {
            _jwtManager = jwtManager;
            _userRepository = userRepository;
        }



        public async Task<TokenPair?> LoginAsync(Login login)
        {
            var user = await _userRepository.LoginAsync(login);
            if (user == null)
                return null;

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.RoleName)
            };

            TokenPair tokenPair = _jwtManager.GenerateTokenPair(claims);
            string hashRefreshToken = _jwtManager.ComputeRefreshHashToken(tokenPair.RefreshToken);

            bool isUpdateToken = await _userRepository.UpdateTokenAsync(hashRefreshToken, user.Email );
            return isUpdateToken ? tokenPair : null;
        }



        public async Task<TokenPair?> RegisterAsync(RegistrationBody user, Roles role = Roles.User)
        {
            string roleName = Enum.GetName(typeof(Roles), role)!;
            var newUser = await _userRepository.AddAsync(user, roleName);
            if (newUser == null)
                return null;

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", newUser.Id.ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };

            TokenPair tokenPair = _jwtManager.GenerateTokenPair(claims);
            string hashRefreshToken = _jwtManager.ComputeRefreshHashToken(tokenPair.RefreshToken);

            bool isUpdateToken = await _userRepository.UpdateTokenAsync(hashRefreshToken, user.Email);
            return isUpdateToken ? tokenPair : null;
        }



        public async Task<TokenPair?> UpdateTokenAsync(TokenPair tokenPair)
        {
            string? email = _jwtManager
                .GetClaimsFromJwt(tokenPair.AccessToken)
                .FirstOrDefault(e => e.ValueType == ClaimTypes.Email)
                ?.Value;

            if (email == null)
                return null;

            var user = await _userRepository.GetAsync(email);
            if (user == null) 
                return null;


            string hashRefreshToken = _jwtManager.ComputeRefreshHashToken(tokenPair.RefreshToken);

            //Одинаковые хэши refresh tokens
            if (user.TokenHash == hashRefreshToken)
            {
                List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, email),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.RoleName)
                    };
                tokenPair.AccessToken = _jwtManager.GenerateAccessToken(claims);

                if (user.TokenValidBefore < DateTime.UtcNow)
                    tokenPair.RefreshToken = _jwtManager.GenerateRefreshToken();
            }

            return tokenPair;
        }
    }
}
