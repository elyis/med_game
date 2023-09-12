using med_game.src.Domain.Enums;

namespace med_game.src.Domain.Entities.Response
{
    public class ProfileBody
    {
        public string Nickname { get; set; } = string.Empty;
        public  string Email { get; set; } = string.Empty;
        public string UrlIcon { get; set; } = string.Empty;
        public Department Department { get; set; } = Department.Anatomy;
    }
}