using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace med_game.src.Entities
{
    public class AnswerOption
    {
        public AnswerOption(TypeAnswer type, string? text, string? image)
        {
            this.type = type;
            this.text = text;
            this.image = image.IsNullOrEmpty() ? "" : image;
        }

        public TypeAnswer type { get; private set; }
        public string? text { get; private set; }
        public string? image { get; private set; }

        public bool Equals(AnswerOption answerOption)
        {
            if( answerOption.type == type && 
                answerOption.image == image &&  
                answerOption.text?.ToLower() == text?.ToLower()
                ) 
                return true;
            return false;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AnswerOption);
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TypeAnswer
    {
        Image,
        Text,
        Input
    }
}
