using System.ComponentModel.DataAnnotations;

namespace med_game.src.Entities.Request
{
    public class RegistrationBody
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Nickname { get; set; }
        public string Password { get; set; }
    }
}
