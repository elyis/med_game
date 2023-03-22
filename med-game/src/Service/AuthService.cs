﻿using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Entities.Request;
using med_game.src.Entities;
using System.Security.Claims;
using med_game.src.Models;
using med_game.src.Utility;
using med_game.src.Core.IManager;

namespace med_game.src.Service
{
    public class AuthService : IAuthService
    {
        private readonly IJwtManager _jwtManager;
        private readonly IUserRepository _userRepository;

        public AuthService(IJwtManager jwtManager, IUserRepository userRepository)
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
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.RoleName)
            };

            TokenPair tokenPair = _jwtManager.GenerateTokenPair(claims);
            string hashRefreshToken = _jwtManager.ComputeRefreshHashToken(tokenPair.Refresh_token);

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
                new Claim("UserId", newUser.Id.ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };

            TokenPair tokenPair = _jwtManager.GenerateTokenPair(claims);
            string hashRefreshToken = _jwtManager.ComputeRefreshHashToken(tokenPair.Refresh_token);

            bool isUpdateToken = await _userRepository.UpdateTokenAsync(hashRefreshToken, user.Mail);
            return isUpdateToken ? tokenPair : null;
        }



        public async Task<TokenPair?> UpdateTokenAsync(string refreshToken)
        {
            string hashRefreshToken = _jwtManager.ComputeRefreshHashToken(refreshToken);
            var user = await _userRepository.GetByToken(hashRefreshToken);
            if(user == null) 
                return null;

            List<Claim> claims = new List<Claim>
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.RoleName)
                };

            TokenPair tokenPair = new TokenPair(
                    accessToken: _jwtManager.GenerateAccessToken(claims),
                    refreshToken: refreshToken
                );

            if (user.TokenValidBefore < DateTime.UtcNow)
                tokenPair.Refresh_token = _jwtManager.GenerateRefreshToken();

            return tokenPair;
        }
    }
}
