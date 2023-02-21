using System.Text.Json.Serialization;

namespace med_game.src.Entities
{
    public class AnswerOption
    {
        public TypeAnswer Type { get; set; }
        public string? Text { get; set; }
        public string? Image { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TypeAnswer
    {
        Image,
        Text,
        Input
    }
}
