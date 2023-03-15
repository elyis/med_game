﻿using med_game.src.Controllers;
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
        public async Task SuccessfulRegisterUser()
        {   
            RegistrationBody registrationBody = new RegistrationBody 
            { 
                Mail = "testy@ss", 
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
            await SuccessfulRegisterUser();

            Login login = new Login
            {
                Mail = "testy@ss",
                Password = "success"
            };

            AuthController authController = new AuthController();
            var result = await authController.SignIn(login) as ObjectResult;
            Assert.NotNull(result);
            
            Assert.Equal((int) HttpStatusCode.OK, (int)result.StatusCode!);
            return (TokenPair) result.Value!;
        }

        [Fact]
        public async Task FailedLogin()
        {
            await SuccessfulRegisterUser();

            Login login = new Login
            {
                Mail = "testy@ss",
                Password = "failed"
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
