using med_game.src.Controllers;
using med_game.src.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace test.Controller
{
    public class LecternControllerTests
    {
        [Fact]
        public async Task SuccessfulCreateLectern()
        {
            LecternController lecternController = new LecternController();

            int[] validStatusCodes = new int[]
            {
                (int) HttpStatusCode.OK,
                (int) HttpStatusCode.Conflict,
            };

            List<LecternBody> lecterns = new List<LecternBody>
            {
                //new LecternBody
                //{
                //    Name = "Акушер-гинеколог",
                //    Description = "Помогает в решении проблем, связанных с зачатием, а также наблюдает за состоянием женщины на протяжении всего периода беременности. Специалист также оказывает необходимые консультации и осуществляет лечение на этапе планирования беременности в послеродовой период"
                //},
                //new LecternBody
                //{
                //    Name = "Аллерголог",
                //    Description = "Специализируется на выявлении аллергических заболеваний, назначает необходимое лечение"
                //},
                //new LecternBody
                //{
                //    Name = "Аллерголог-иммунолог",
                //    Description = "Специализируется на выявлении и лечении аллергических заболеваний, а также нарушений иммунной системы",
                //},
                //new LecternBody
                //{
                //    Name = "Ангиохирург",
                //    Description = "Специализируется на хирургическом лечении заболеваний сосудистой системы. К нему обращаются в том случае, если появляются следующие симптомы: боли в ногах при нагрузке, сосудистые «звёздочки» на коже ног, отеки, тяжесть в ногах"
                //},
                new LecternBody
                {
                    Name = "Анатомия",
                    Description = "Изучает внутреннее строение тела"
                },
            };

            var result = await lecternController.CreateLectern(lecterns) as OkResult;
            Assert.NotNull(result);

            Assert.Contains(result.StatusCode!, validStatusCodes);
        }


        [Fact]
        public async Task SuccessfulRemoveLectern()
        {
            await SuccessfulCreateLectern();

            LecternController lecternController = new LecternController();
            int responseStatusCode = (int) HttpStatusCode.NoContent;

            string[] removedLecterns = new string[]
            {
                "Акушер-гинеколог",
                "Аллерголог",
                "Аллерголог-иммунолог",
                "Ангиохирург"
            };

            foreach(string lectern in removedLecterns)
            {
                var result = await lecternController.RemoveLectern(lectern) as StatusCodeResult;
                Assert.NotNull(result);

                if (result.StatusCode != (int)HttpStatusCode.NoContent)
                    responseStatusCode = result.StatusCode!;
            }

            Assert.Equal((int)HttpStatusCode.NoContent, responseStatusCode);
        }

        [Fact]
        public async Task RemoveLecternIfNotExist()
        {
            LecternController lecternController = new LecternController();
            int responseStatusCode = (int)HttpStatusCode.NoContent;

            int[] validStatusCodes = new int[]
            {
                (int) HttpStatusCode.NoContent,
                (int) HttpStatusCode.NotFound,
            };

            string[] removedLecterns = new string[]
            {
                "Not exist",
                "i'm superman",
                "Аллерголог-иммунолог",
                "Ангиохирург"
            };

            foreach(string lectern in removedLecterns)
            {
                var result = await lecternController.RemoveLectern(lectern) as StatusCodeResult;
                Assert.NotNull(result);

                if (result.StatusCode != (int)HttpStatusCode.NoContent)
                    responseStatusCode = result.StatusCode;
            }

            Assert.Contains(responseStatusCode, validStatusCodes);
        }
    }
}
