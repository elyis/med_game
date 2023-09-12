namespace med_game.src.Domain.Entities.Response
{
    public class GameStatistics
    {
        public GameStatistics(string nickname, string? image)
        {
            this.Nickname = nickname;
            this.Image = image;
        }

        public string Nickname { get; set; }
        public string? Image { get; set; }
        public int CountOfPoints { get; set; } = 0;
        public int PointGain { get; set; } = 0;
        public int MaxPointsGame { get; set; } = 0;
    }

    public class StateGame
    {
        public bool IsEndGame { get; set; }
        public List<GameStatistics> Rating { get; set;} = new();
        public string? NameWinner { get; set; }
    }

}