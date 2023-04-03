using med_game.src.Controllers;
using med_game.src.Data;
using med_game.src.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography.Xml;
using Xunit;

namespace test.Controller
{
    public class AchievementControllerTests
    {
        [Fact]
        public async Task SuccessfulCreateAchievement()
        {
            AchievementController controller = new AchievementController(new AppDbContext(new DbContextOptions<AppDbContext>()));
            AchievementBody achievementBody = new AchievementBody
            {
                Name = "Победитель",
                Description = "Победить в 10 играх",
                CountPoints = 0,
                MaxCountPoints = 10
            };

            int[] validHttpCodes = new int[]
            {
                (int) HttpStatusCode.OK,
                (int) HttpStatusCode.Conflict,
            };

            var result = await controller.CreateAchievement(achievementBody) as StatusCodeResult;
            Assert.NotNull(result);
            Assert.Contains(result.StatusCode, validHttpCodes);
        }


        [Fact]
        public async Task SuccesfulRemoveAchievement()
        {
            AchievementController controller = new AchievementController(new AppDbContext(new DbContextOptions<AppDbContext>()));
            int responseStatusCode = (int)HttpStatusCode.NoContent;

            string[] removedAchievementNames = new string[]
            {
                "Победитель",
            };

            await SuccessfulCreateAchievement();

            foreach(string name in removedAchievementNames)
            {
                var result = await controller.RemoveAchievementByName(name) as StatusCodeResult;
                Assert.NotNull(result);

                if (result.StatusCode != (int) HttpStatusCode.NoContent)
                    responseStatusCode = result.StatusCode;
            }

            Assert.Equal((int) HttpStatusCode.NoContent, responseStatusCode);
        }


        [Fact]
        public async Task RemoveAchievementIfNotExist()
        {
            AchievementController controller = new AchievementController(new AppDbContext(new DbContextOptions<AppDbContext>()));
            int responseStatusCode = (int)HttpStatusCode.NoContent;

            int[] validStatusCodes = new int[]
            {
                (int) HttpStatusCode.NoContent,
                (int) HttpStatusCode.NotFound
            };

            string[] removedAchievementNames = new string[]
            {
                "Победитель",
                "No exist"
            };

            foreach (string name in removedAchievementNames)
            {
                var result = await controller.RemoveAchievementByName(name) as StatusCodeResult;
                Assert.NotNull(result);

                if (result.StatusCode != (int)HttpStatusCode.NoContent)
                    responseStatusCode = result.StatusCode;
            }

            Assert.Contains(responseStatusCode, validStatusCodes);
        }
    }
}
