using System.Text.Json.Serialization;

namespace med_game.src.Entities
{
    public class AnswerOption
    {
        public TypeAnswer Type { get; set; }
        public string? Text { get; set; }
        public string? Image { get; set; }

        public bool Equals(AnswerOption answerOption)
        {
            if( answerOption.Type == Type && 
                answerOption.Image == Image && 
                answerOption.Text == Text
                ) 
                return true;
            return false;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AnswerOption);
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TypeAnswer
    {
        Image,
        Text,
        Input
    }
}
