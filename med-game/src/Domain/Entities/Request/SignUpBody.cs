using System.ComponentModel.DataAnnotations;

namespace med_game.src.Domain.Entities.Request
{
    public class SignUpBody
    {
        [EmailAddress]
        public string Mail { get; set; }

        public string Nickname { get; set; }
        public string Password { get; set; }
    }
}