using med_game.src.Data;
using med_game.src.models;
using med_game.src.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace med_game.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Test : ControllerBase
    {
        [HttpGet]
        public async Task Get()
        {
            using (var context = new AppDbContext())
            {
                var users = new User[]
                {
                    new User { Email = "1@1", Nickname = "1", Password = "password", RoleName = "User" },

                    new User { Email = "2@2", Nickname = "2", Password = "password", RoleName = "User" },

                    new User { Email = "3@3", Nickname = "3", Password = "password", RoleName = "User" },
                };

                users[0].Friends.AddRange(new List<Friend> { new Friend { Subscriber = users[1] }, new Friend { Subscriber = users[2] } });



                JsonSerializerOptions options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, WriteIndented = true };
                var result = new List<User> { users[1], users[2] };

                context.Users.AddRange(users);
                context.SaveChanges();

                await Response.WriteAsJsonAsync(users, options);
            }

        }
    }

    public class Testy
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<DbTesty> dbTesties { get; set; } = new List<DbTesty>();
    }

    public class DbTesty
    {
        public int TestyId { get; set; }
        public Testy Testy { get; set; }
        public int SubId { get; set; }
        public Testy Sub { get; set; }
    }

}
