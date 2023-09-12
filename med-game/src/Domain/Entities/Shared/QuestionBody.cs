using med_game.src.Domain.Enums;

namespace med_game.src.Domain.Entities.Shared
{
    public class QuestionBody
    {
        public TypeQuestion type { get; set; }
        public string? text { get; set; }
        public string? description { get; set; }
        public string? image { get; set; }
        public int timeSeconds { get; set; }
        public int numOfPointsPerAnswer { get; set; }
        public AnswerOption RightAnswer { get; set; }
        public List<AnswerOption> Answers { get; set; }
    }

    public class QuestionProperties
    {
        public TypeQuestion Type { get; set;}
        public string? Text { get; set;}
        public string? Description { get; set;}
        public string? Image { get; set; }
    }
}