using med_game.src.Controllers;
using med_game.src.Entities;
using med_game.src.Entities.Request;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace test.Controller
{
    public class AuthControllerTests
    {
        [Fact]
        public async void SuccessfulRegisterUser()
        {   
            RegistrationBody registrationBody = new RegistrationBody 
            { 
                Email = "testy@ss", 
                Nickname = "testy", 
                Password = "success" 
            };

            int[] validStatusCodes = new int[]
            {
                (int) HttpStatusCode.OK,
                (int) HttpStatusCode.Conflict,
            };

            AuthController authController = new AuthController();

            var result = await authController.SignUp(registrationBody) as ObjectResult;
            Assert.NotNull(result);

            Assert.Contains((int) result.StatusCode!, validStatusCodes);
        }

        [Fact]
        public async Task<TokenPair> SuccessfulLogin()
        {
            SuccessfulRegisterUser();

            Login login = new Login
            {
                Email = "testy@ss",
                PasswordHash = "success"
            };

            AuthController authController = new AuthController();
            var result = await authController.SignIn(login) as ObjectResult;
            Assert.NotNull(result);
            
            Assert.Equal((int) HttpStatusCode.OK, (int)result.StatusCode!);
            return (TokenPair) result.Value!;
        }

        [Fact]
        public async void FailedLogin()
        {
            SuccessfulRegisterUser();

            Login login = new Login
            {
                Email = "testy@ss",
                PasswordHash = "failed"
            };

            AuthController authController = new AuthController();


            var result = await authController.SignIn(login) as ObjectResult;
            Assert.Null(result);
        }

        [Fact]
        public async Task<TokenPair> SuccessRestoreTokenPair()
        {
            var tokenPair = await SuccessfulLogin();
            Assert.NotNull(tokenPair);

            var authController = new AuthController();
            var result = await authController.RestoreToken(tokenPair) as ObjectResult;
            Assert.NotNull(result);

            Assert.Equal((int)HttpStatusCode.OK, (int)result.StatusCode!);
            return (TokenPair) result.Value!;
        }

    }
}
