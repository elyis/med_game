
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace med_game.src.Entities
{
    public class QuestionBody
    {
        public TypeQuestion type { get; set; }
        public string? text { get; set; }
        public string? description { get; set; }
        public string? image { get; set; }
        public int timeSeconds { get; set; }
        public int numOfPointsPerAnswer { get; set; }
        public AnswerOption rightAnswer { get; set; }
        public List<AnswerOption> answers { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TypeQuestion
    {
        Image,
        Text,
    }

    public class QuestionProperties
    {
        public TypeQuestion Type { get; set;}
        public string? Text { get; set;}
        public string? Description { get; set;}
        public string? Image { get; set; }
    }
}
