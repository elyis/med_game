
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
                answerOption.Text?.ToLower() == Text?.ToLower()
                ) 
                return true;
            return false;
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TypeAnswer
    {
        Image,
        Text,
    }
}
