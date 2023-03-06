namespace med_game.src.Entities.Response
{
    public class ProfileBody
    {
        public string Nickname { get; set; }
        public  string Email { get; set; }
        public string? UrlIcon { get; set; }
        public List<AchievementBody> Achievements { get; set; } = new();
        public Department Department { get; set; } = Department.Anatomy;
    }
}
