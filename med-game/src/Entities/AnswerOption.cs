
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace med_game.src.Entities
{
    public class AnswerOption
    {
        public TypeAnswer type { get; set; }
        public string? text { get; set; }
        public string? image { get; set; }

        public bool Equals(AnswerOption answerOption)
        {
            if( answerOption.type == type && 
                ((answerOption.image == image) || (answerOption.image.IsNullOrEmpty() && image.IsNullOrEmpty())) &&  
                answerOption.text?.ToLower() == text?.ToLower()
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
        Input
    }
}
