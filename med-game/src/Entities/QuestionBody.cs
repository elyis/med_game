using System.Text.Json.Serialization;

namespace med_game.src.Entities
{
    public class QuestionBody
    {
        public TypeQuestion Type { get; set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int TimeSeconds { get; set; }
        public int NumOfPointsPerAnswer { get; set; }
        public AnswerOption RightAnswer { get; set; }
        public List<AnswerOption> Answers { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TypeQuestion
    {
        Image,
        Text
    }

    public class QuestionProperties
    {
        public TypeQuestion Type { get; set;}
        public string? Text { get; set;}
        public string? Description { get; set;}
        public string? Image { get; set; }
    }
}
