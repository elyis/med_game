using med_game.src.Domain.Enums;
using med_game.src.Domain.Models;

namespace med_game.src.Domain.Entities.Shared
{
    public class AnswerOption
    {
        public AnswerOption(TypeAnswer type, string? text, string? image)
        {
            this.Type = type;
            this.Text = text;
            this.Image = (image == "" || image == null) ? "" : image;
        }

        public TypeAnswer Type { get; private set; }
        public string? Text { get; private set; }
        public string? Image { get; private set; }

        public bool Equals(AnswerOption answerOption)
        {
            if( answerOption.Type == Type && 
                answerOption.Image == Image &&  
                answerOption.Text?.ToLower() == Text?.ToLower()
                ) 
                return true;
            return false;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AnswerOption);
        }

        public AnswerModel ToAnswerModel(){
            return new AnswerModel{
                Description = Text,
                Image = Image,
                Type = Enum.GetName(typeof(TypeAnswer), Type)!
            };
        }
    }
}