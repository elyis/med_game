using med_game.Models;
using Microsoft.EntityFrameworkCore;

namespace med_game.src.Models
{
    [Index("Email", IsUnique = true)]
    public class User
    {
        public long Id { get; private set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public int Rating { get; set; }
        public string? Image { get; set; }
        public string? TokenHash { get; set; }
        public DateTime TokenValidBefore { get; set; }
        public List<Achievement> Achievements { get; set; } = new();
    }
}
