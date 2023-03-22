namespace med_game.src.Entities.Response
{
    public class GameQuestion
    {
        public TypeQuestion type { get; set; }
        public string? text { get; set; }
        public string? description { get; set; }
        public string? image { get; set; }
        public int timeSeconds { get; set; }
        public AnswerOption rightAnswer { get; set; }
        public List<AnswerOption> answers { get; set; }
    }
}
