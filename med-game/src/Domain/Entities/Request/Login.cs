using System.ComponentModel.DataAnnotations;

namespace med_game.src.Domain.Entities.Request
{
    public class Login
    {
        [EmailAddress]
        public string Mail { get; set; }

        public string Password { get; set; }
    }
}