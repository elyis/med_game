namespace med_game.src.Entities.Response
{
    public class GameStatistics
    {
        public GameStatistics(string nickname, string? image)
        {
            this.nickname = nickname;
            this.image = image;
        }

        public string nickname { get; set; }
        public string? image { get; set; }
        public int countOfPoints { get; set; } = 0;
        public int pointGain { get; set; } = 0;
        public int maxPointsGame { get; set; } = 0;
    }

    public class StateGame
    {
        public bool isEndGame { get; set; }
        public List<GameStatistics> rating { get; set;}
        public string? nameWinner { get; set; }
    }
}
