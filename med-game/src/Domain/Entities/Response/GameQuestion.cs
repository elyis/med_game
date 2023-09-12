using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Enums;

namespace med_game.src.Domain.Entities.Response
{
    public class GameQuestion
    {
        public TypeQuestion type { get; set; }
        public string? text { get; set; }
        public string? description { get; set; }
        public string? image { get; set; }
        public int timeSeconds { get; set; }
        public AnswerOption RightAnswer { get; set; }
        public List<AnswerOption> Answers { get; set; }
    }
}