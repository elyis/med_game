using med_game.src.Controllers;
using med_game.src.Data;
using med_game.src.Entities.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace test.Controller
{
    public class ModuleControllerTests
    {
        [Fact]
        public async Task SuccesfulCreateModule()
        {
            ModuleController controller = new ModuleController(new AppDbContext(new DbContextOptions<AppDbContext>()));
            string lecternName = "Anatomy";

            RequestedModuleBody[] moduleBodies = new RequestedModuleBody[]
            {
                new RequestedModuleBody { LecternName = lecternName, ModuleName = "Остеология"},
                new RequestedModuleBody { LecternName = lecternName, ModuleName = "Миология"},
                new RequestedModuleBody { LecternName = lecternName, ModuleName = "Спланхнология"},
                new RequestedModuleBody { LecternName = lecternName, ModuleName = "Центральная нервная система"},
                new RequestedModuleBody { LecternName = lecternName, ModuleName = "Периферическая нервная система"},
                new RequestedModuleBody { LecternName = lecternName, ModuleName = "Сердечно-сосудистая система"},
                new RequestedModuleBody { LecternName = lecternName, ModuleName = "Интересные факты"}
            };


            foreach(var moduleBody in moduleBodies)
            {
                var result = await controller.CreateModule(moduleBody) as StatusCodeResult;
                Assert.NotNull(result);
                Assert.NotEqual((int)HttpStatusCode.NotFound, result.StatusCode);
            }
        }

        [Fact]
        public async Task SuccessfulRemoveModule()
        {
            await SuccesfulCreateModule();

            ModuleController controller = new ModuleController(new AppDbContext(new DbContextOptions<AppDbContext>()));
            int responseStatusCode = (int)HttpStatusCode.NoContent;

            string lecternName = "Anatomy";

            RemovableModuleBody[] moduleBodies = new RemovableModuleBody[]
            {
                new RemovableModuleBody { LecternName = lecternName, ModuleName = "Остеология"},
                new RemovableModuleBody { LecternName = lecternName, ModuleName = "Миология"},
                new RemovableModuleBody { LecternName = lecternName, ModuleName = "Спланхнология"},
                new RemovableModuleBody { LecternName = lecternName, ModuleName = "Центральная неврная система"},
                new RemovableModuleBody { LecternName = lecternName, ModuleName = "Периферическая нервная система"},
                new RemovableModuleBody { LecternName = lecternName, ModuleName = "Сердечно-сосудистая система"},
                new RemovableModuleBody { LecternName = lecternName, ModuleName = "Интересные факты"}
            };

            foreach (var moduleBody in moduleBodies)
            {
                var result = await controller.RemoveModule(moduleBody) as StatusCodeResult;
                Assert.NotNull(result);

                if (result.StatusCode != (int)HttpStatusCode.NoContent)
                    responseStatusCode = result.StatusCode;
            }

            Assert.Equal((int)HttpStatusCode.NoContent, responseStatusCode);
        }


    }
}
