namespace med_game.src.Entities.Response
{
    public class GameStatistics
    {
        public GameStatistics(string nickname, string? image)
        {
            Nickname = nickname;
            Image = image;
        }

        public string Nickname { get; set; }
        public string? Image { get; set; }
        public int CountPoints { get; set; } = 0;
        public int PointGain { get; set; } = 0;
        public int MaxPoints { get; set; } = 0;
    }

    public class StateGame
    {
        public bool IsEndGame { get; set; }
        public List<GameStatistics> Statistics { get; set;}
        public string? WinnerName { get; set; }
    }
}
