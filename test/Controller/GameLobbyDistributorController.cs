using med_game.src.Controllers;
using med_game.src.Entities.Request;

using Xunit;

namespace test.Controller
{
    public class GameLobbyDistributorController
    {
        [Fact]
        public async Task LobbyDistributorTesting()
        {
            List<RegistrationBody> registrations = new List<RegistrationBody>();
            List<AuthController> authControllers = new List<AuthController>();
            int countUsers = 100;
            int countThreads = 6;

            for(int i = 0; i < countUsers; i++)
            {
                registrations.Add(
                        new RegistrationBody { Email = "testy@" + i, Nickname = "testy" + i, Password = "password" }
                    );
                authControllers.Add(new AuthController());
            }
            



        }


    }
}
